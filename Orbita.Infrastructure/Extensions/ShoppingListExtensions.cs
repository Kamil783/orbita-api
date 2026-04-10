using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class ShoppingListExtensions
{
    public static ShoppingListEntity ToEntity(this ShoppingList list)
    {
        return new ShoppingListEntity
        {
            Id = list.Id.Id,
            CreatorId = list.CreatorId.Id,
            TeamId = list.TeamId.Id,
            Name = list.Name,
            IsFromBalance = list.IsFromBalance,
            Pinned = list.Pinned,
            CreatedAt = list.CreatedAt,
            Items = list.Items.Select(i => i.ToEntity()).ToList()
        };
    }

    public static ShoppingList ToDomain(this ShoppingListEntity entity)
    {
        return ShoppingList.Restore(
            id: new ShoppingListId(entity.Id),
            creatorId: new UserId(entity.CreatorId),
            teamId: new TeamId(entity.TeamId),
            name: entity.Name,
            isFromBalance: entity.IsFromBalance,
            pinned: entity.Pinned,
            createdAt: entity.CreatedAt,
            items: entity.Items.Select(i => i.ToDomain()).ToList()
        );
    }

    public static ShoppingListItemEntity ToEntity(this ShoppingListItem item)
    {
        return new ShoppingListItemEntity
        {
            Id = item.Id.Id,
            ListId = item.ListId.Id,
            FinanceTransactionId = item.FinanceTransactionId?.Id,
            Name = item.Name,
            Price = item.Price,
            Bought = item.Bought,
            Order = item.Order
        };
    }

    public static ShoppingListItem ToDomain(this ShoppingListItemEntity entity)
    {
        return ShoppingListItem.Restore(
            id: new ShoppingListItemId(entity.Id),
            listId: new ShoppingListId(entity.ListId),
            financeTransactionId: entity.FinanceTransactionId.HasValue ? new FinanceTransactionId(entity.FinanceTransactionId.Value) : null,
            name: entity.Name,
            price: entity.Price,
            bought: entity.Bought,
            order: entity.Order
        );
    }
}
