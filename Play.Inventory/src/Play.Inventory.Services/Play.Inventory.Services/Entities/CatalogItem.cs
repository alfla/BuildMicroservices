using Play.Catalog.Service.Entities;

namespace Play.Inventory.Services.Dtos.Entities;

public class CatalogItem : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}