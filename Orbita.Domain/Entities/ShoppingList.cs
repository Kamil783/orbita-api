using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class ShoppingList
{
    public ShoppingListId Id { get; private set; }
    public UserId CreatorId { get; private set; }
    public TeamId TeamId { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<ShoppingListItem> Items { get; private set; } = new();

    private ShoppingList() { }

    public static ShoppingList Create(
        UserId creatorId,
        TeamId teamId,
        string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        return new ShoppingList
        {
            Id = new ShoppingListId(Guid.NewGuid()),
            CreatorId = creatorId,
            TeamId = teamId,
            Name = name,
            CreatedAt = DateTime.UtcNow,
            Items = new List<ShoppingListItem>()
        };
    }

    public static ShoppingList Restore(
        ShoppingListId id,
        UserId creatorId,
        TeamId teamId,
        string name,
        DateTime createdAt,
        List<ShoppingListItem> items)
    {
        return new ShoppingList
        {
            Id = id,
            CreatorId = creatorId,
            TeamId = teamId,
            Name = name,
            CreatedAt = createdAt,
            Items = items
        };
    }
}
