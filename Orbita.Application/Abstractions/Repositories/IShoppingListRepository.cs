using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IShoppingListRepository
{
    Task<List<ShoppingList>> GetByTeamAsync(Guid teamId, CancellationToken ct = default);
    Task<ShoppingList?> GetAsync(Guid id, CancellationToken ct = default);
    Task<ShoppingList> CreateAsync(ShoppingList list, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<ShoppingListItem> AddItemAsync(ShoppingListItem item, CancellationToken ct = default);
    Task<ShoppingListItem?> GetItemAsync(Guid itemId, CancellationToken ct = default);
    Task<ShoppingListItem> UpdateItemAsync(ShoppingListItem item, CancellationToken ct = default);
    Task DeleteItemAsync(Guid itemId, CancellationToken ct = default);
}
