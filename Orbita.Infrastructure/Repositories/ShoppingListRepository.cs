using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class ShoppingListRepository(OrbitaDbContext db) : IShoppingListRepository
{
    public async Task<List<ShoppingList>> GetByTeamAsync(Guid teamId, CancellationToken ct = default)
    {
        var entities = await db.ShoppingLists
            .Include(x => x.Items)
            .Where(x => x.TeamId == teamId)
            .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<ShoppingList?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.ShoppingLists
            .Include(x => x.Items)
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
}
