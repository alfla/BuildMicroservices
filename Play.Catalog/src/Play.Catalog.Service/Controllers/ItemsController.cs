using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Dtos.Extentions;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> _itemRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public ItemsController(IRepository<Item> itemRepository, IPublishEndpoint publishEndpoint)
        {
            _itemRepository = itemRepository;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            var items = (await _itemRepository.GetAllAsync())
                .Select(item => item.AsItemDto());
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await _itemRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }
            return item.AsItemDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = createItemDto.AsItem();
            await _itemRepository.CreateAsync(item);
            await _publishEndpoint.Publish(new Contracts.Contracts.CatalogItemCreated(item.Id, item.Name, item.Description));
            var actionName = nameof(GetByIdAsync);
            return CreatedAtAction(actionName, new { id = item.Id }, item);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await _itemRepository.GetAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await _itemRepository.UpdateAsync(existingItem);
            await _publishEndpoint.Publish(new Contracts.Contracts.CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _itemRepository.DeleteAsync(id);
            await _publishEndpoint.Publish(new Contracts.Contracts.CatalogItemDeleted(id));
            return NoContent();
        }
        
    }
}