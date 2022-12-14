using Play.Catalog.Contracts;
using MassTransit;
using Play.Common;
using Play.Inventory.Services.Dtos.Entities;

namespace Play.Inventory.Services.Consumers;

public class CatalogItemCreatedConsumer : IConsumer<Contracts.CatalogItemCreated>
{
    private readonly IRepository<CatalogItem> _repository;

    public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<Contracts.CatalogItemCreated> context)
    {
        var message = context.Message;
        var item = await _repository.GetAsync(message.ItemId);
        
        if(item != null)
            return;
        item = new CatalogItem
        {
            Id = message.ItemId,
            Name = message.Name,
            Description = message.Description
        };
        
        await _repository.CreateAsync(item);
    }
}