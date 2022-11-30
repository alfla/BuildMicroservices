using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Dtos.Extentions;

public static class Converter
{
    public static ItemDto AsItemDto(this Item item)
    {
        return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
    }

    public static Item AsItem(this CreateItemDto itemDto)
    {
        return new Item
        {
            Id = Guid.NewGuid(),
            Name = itemDto.Name,
            Description = itemDto.Description,
            Price = itemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };
    }

    public static Item AsItem(this UpdateItemDto itemDto, ItemDto item)
    {
        return new Item
        {
            Id = item.Id,
            Name = itemDto.Name,
            Description = itemDto.Description,
            Price = itemDto.Price,
            CreatedDate = item.CreatedDate
        };
    }
}