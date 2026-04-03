using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class ShoppingListItem
{
    public ShoppingListItemId Id { get; private set; }
    public ShoppingListId ListId { get; private set; }
    public string Name { get; private set; }
    public long? Price { get; private set; }
    public bool Bought { get; private set; }

    private ShoppingListItem() { }

    public static ShoppingListItem Create(
        ShoppingListId listId,
        string name,
        long? price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (price is < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price must be non-negative.");

        return new ShoppingListItem
        {
            Id = new ShoppingListItemId(Guid.NewGuid()),
            ListId = listId,
            Name = name,
            Price = price,
            Bought = false
        };
    }

    public static ShoppingListItem Restore(
        ShoppingListItemId id,
        ShoppingListId listId,
        string name,
        long? price,
        bool bought)
    {
        return new ShoppingListItem
        {
            Id = id,
            ListId = listId,
            Name = name,
            Price = price,
            Bought = bought
        };
    }

    public long ChangeBoughtStatus(bool bought)
    {
        var price = Price ?? 0L;

        if (Bought == bought)
            return 0;

        var delta = 0L;

        if (!Bought && bought)
            delta = -price;
        else if (Bought && !bought)
            delta = price;

        Bought = bought;
        return delta;
    }
}
