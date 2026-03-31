namespace Orbita.Infrastructure.Entities;

public class ShoppingListItemEntity
{
    public Guid Id { get; set; }
    public Guid ListId { get; set; }
    public string Name { get; set; } = default!;
    public long? Price { get; set; }
    public bool Bought { get; set; }

    public ShoppingListEntity List { get; set; } = default!;
}
