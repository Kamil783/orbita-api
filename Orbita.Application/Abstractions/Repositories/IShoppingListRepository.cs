using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IShoppingListRepository
{
    Task<List<ShoppingList>> GetForUserAsync(Guid teamId, Guid creatorId, CancellationToken ct = default);
    Task<ShoppingList?> GetAsync(Guid id, CancellationToken ct = default);
    Task<ShoppingList> CreateAsync(ShoppingList list, CancellationToken ct = default);
    Task<ShoppingList> UpdateAsync(ShoppingList list, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<ShoppingListItem> AddItemAsync(ShoppingListItem item, CancellationToken ct = default);
    Task<ShoppingListItem?> GetItemAsync(Guid itemId, CancellationToken ct = default);
    Task<ShoppingListItem> UpdateItemAsync(ShoppingListItem item, CancellationToken ct = default);
    Task DeleteItemAsync(Guid itemId, CancellationToken ct = default);
    Task<int> GetMaxItemOrderAsync(Guid listId, CancellationToken ct = default);
    Task ReorderItemsAsync(Guid listId, List<Guid> itemIds, CancellationToken ct = default);
}
