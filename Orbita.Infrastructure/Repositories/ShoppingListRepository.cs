using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class ShoppingListRepository(OrbitaDbContext db) : IShoppingListRepository
{
    public async Task<List<ShoppingList>> GetForUserAsync(Guid teamId, Guid creatorId, CancellationToken ct = default)
    {
        var entities = await db.ShoppingLists
            .Include(x => x.Items.OrderBy(i => i.Order))
            .Where(x => x.TeamId == teamId)
        .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<ShoppingList?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.ShoppingLists
            .Include(x => x.Items.OrderBy(i => i.Order))
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return entity?.ToDomain();
    }

    public async Task<ShoppingList> CreateAsync(ShoppingList list, CancellationToken ct = default)
    {
        var entity = list.ToEntity();
        await db.ShoppingLists.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task<ShoppingList> UpdateAsync(ShoppingList list, CancellationToken ct = default)
    {
        var entity = await db.ShoppingLists
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == list.Id.Id, ct);
        if (entity is null) throw new InvalidOperationException("Shopping list not found.");

        entity.Name = list.Name;
        entity.Pinned = list.Pinned;
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.ShoppingLists.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            db.ShoppingLists.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task<ShoppingListItem> AddItemAsync(ShoppingListItem item, CancellationToken ct = default)
    {
        var entity = item.ToEntity();
        await db.ShoppingListItems.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task<ShoppingListItem?> GetItemAsync(Guid itemId, CancellationToken ct = default)
    {
        var entity = await db.ShoppingListItems.FirstOrDefaultAsync(x => x.Id == itemId, ct);
        return entity?.ToDomain();
    }

    public async Task<ShoppingListItem> UpdateItemAsync(ShoppingListItem item, CancellationToken ct = default)
    {
        var entity = await db.ShoppingListItems.FirstOrDefaultAsync(x => x.Id == item.Id.Id, ct);
        if (entity is null) throw new InvalidOperationException("Shopping list item not found.");

        entity.Name = item.Name;
        entity.Price = item.Price;
        entity.Bought = item.Bought;
        entity.Order = item.Order;
        entity.FinanceTransactionId = item.FinanceTransactionId?.Id;
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task DeleteItemAsync(Guid itemId, CancellationToken ct = default)
    {
        var entity = await db.ShoppingListItems.FirstOrDefaultAsync(x => x.Id == itemId, ct);
        if (entity is not null)
        {
            db.ShoppingListItems.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task<int> GetMaxItemOrderAsync(Guid listId, CancellationToken ct = default)
    {
        var any = await db.ShoppingListItems.AnyAsync(x => x.ListId == listId, ct);
        if (!any) return -1;
        return await db.ShoppingListItems
            .Where(x => x.ListId == listId)
            .MaxAsync(x => x.Order, ct);
    }

    public async Task ReorderItemsAsync(Guid listId, List<Guid> itemIds, CancellationToken ct = default)
    {
        var entities = await db.ShoppingListItems
            .Where(x => x.ListId == listId)
            .ToListAsync(ct);

        var lookup = entities.ToDictionary(e => e.Id);

        for (var i = 0; i < itemIds.Count; i++)
        {
            if (lookup.TryGetValue(itemIds[i], out var entity))
                entity.Order = i;
        }

        await db.SaveChangesAsync(ct);
    }
}
