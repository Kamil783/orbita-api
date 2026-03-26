using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface ITodoItemRepository
{
    Task<TodoItem?> GetAsync(Guid id, CancellationToken ct = default);
    Task<TodoItem?> GetByBacklogIdAsync(Guid backlogId, CancellationToken ct = default);
    Task<TodoItem> CreateAsync(TodoItem item, CancellationToken ct = default);
    Task<TodoItem?> UpdateAsync(TodoItem item, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<int> GetMaxSortOrderAsync(Guid columnId, CancellationToken ct = default);

    /// <summary>
    /// Moves a card between columns (or within a column) with exact index-based positioning.
    /// Returns false if the card at fromIndex does not match taskId (conflict).
    /// </summary>
    Task<bool> MoveCardAsync(Guid taskId, Guid fromColumnId, Guid toColumnId, int fromIndex, int toIndex, CancellationToken ct = default);

    /// <summary>
    /// Moves a card to the end of the target column.
    /// </summary>
    Task MoveCardToEndAsync(Guid taskId, Guid targetColumnId, CancellationToken ct = default);
}
