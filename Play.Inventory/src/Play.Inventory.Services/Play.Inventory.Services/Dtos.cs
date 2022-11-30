namespace Play.Inventory.Services.Dtos;

public record GrantItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);
public record InventoryItemsDto(Guid CatalogItemId, int Quantity, string Name, string Description, DateTimeOffset AcquiredDate);
public record CatalogItemDto(Guid Id, string Name, string Description);
