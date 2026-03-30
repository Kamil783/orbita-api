namespace Orbita.Infrastructure.Entities;

public class ShoppingListEntity
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public Guid TeamId { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public List<ShoppingListItemEntity> Items { get; set; } = new();
}
