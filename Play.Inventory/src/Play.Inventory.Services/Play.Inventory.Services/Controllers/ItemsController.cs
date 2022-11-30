using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Services.Client;
using Play.Inventory.Services.Dtos.Entities;
using Play.Inventory.Services.Dtos.Extentions;

namespace Play.Inventory.Services.Dtos.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<InventoryItem> _itemsRepository;

    private readonly CatalogClient _catalogClient;
    // GET
    public ItemsController(IRepository<InventoryItem> itemsRepository, CatalogClient catalogClient)
    {
        _itemsRepository = itemsRepository;
        _catalogClient = catalogClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemsDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest();
        }

        var catalogItems = await _catalogClient.GetCatalogItemAsync();
        var inventoryItemsEntities = await _itemsRepository.GetAllAsync(item => item.UserId == userId);

        var inventoryItemsDtos = inventoryItemsEntities.Select(inventoryItem =>
        {
            var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
            return inventoryItem.AsDto(catalogItem);
        });
        
        return Ok(inventoryItemsDtos);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
    {
        var inventoryItem = await _itemsRepository.GetAsync(item =>
            item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);
        if (inventoryItem == null)
        {
            inventoryItem = grantItemsDto.AsInventoryItem();
            await _itemsRepository.CreateAsync(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += grantItemsDto.Quantity;
            await _itemsRepository.UpdateAsync(inventoryItem);
        }

        return Ok();
    }
}