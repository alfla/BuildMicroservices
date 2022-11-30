using Play.Inventory.Services.Dtos.Entities;

namespace Play.Inventory.Services.Dtos.Extentions;

public static class Converters
{
    public static InventoryItemsDto AsDto(this InventoryItem item, CatalogItemDto catalogItemDto)
    {
        return new InventoryItemsDto(item.Id, item.Quantity, catalogItemDto.Name, catalogItemDto.Description, item.AcquiredDate);
    }

    public static InventoryItem AsInventoryItem(this GrantItemsDto itemsDto)
    {
        return new InventoryItem
        {
            Id = Guid.NewGuid(),
            UserId = itemsDto.UserId,
            CatalogItemId = itemsDto.CatalogItemId,
            Quantity = itemsDto.Quantity,
            AcquiredDate = DateTimeOffset.UtcNow
        };
    }
}