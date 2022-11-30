using Play.Catalog.Contracts;
using MassTransit;
using Play.Common;
using Play.Inventory.Services.Dtos.Entities;

namespace Play.Inventory.Services.Consumers;

public class CatalogItemDeletedConsumer : IConsumer<Contracts.CatalogItemCreated>
{
    private readonly IRepository<CatalogItem> _repository;

    public CatalogItemDeletedConsumer(IRepository<CatalogItem> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<Contracts.CatalogItemCreated> context)
    {
        var message = context.Message;
        var item = await _repository.GetAsync(message.ItemId);

        if (item != null)
        {
            await _repository.DeleteAsync(item.Id);
        }
    }
}