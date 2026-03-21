using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class TodoItemRepository(OrbitaDbContext db) : ITodoItemRepository
{
    public async Task<TodoItem?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.TodoItems
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return entity?.ToDomain();
    }

    public async Task<TodoItem?> GetByBacklogIdAsync(Guid backlogId, CancellationToken ct = default)
    {
        var entity = await db.TodoItems
            .FirstOrDefaultAsync(x => x.BacklogId == backlogId, ct);

        return entity?.ToDomain();
    }

    public async Task<TodoItem> CreateAsync(TodoItem item, CancellationToken ct = default)
    {
        var entity = item.ToEntity();

        await db.TodoItems.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);

        return entity.ToDomain();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.TodoItems.FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity is not null)
        {
            db.TodoItems.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task<int> GetMaxSortOrderAsync(Guid columnId, CancellationToken ct = default)
    {
        var hasItems = await db.TodoItems.AnyAsync(x => x.ColumnId == columnId, ct);
        if (!hasItems)
            return -1;

        return await db.TodoItems
            .Where(x => x.ColumnId == columnId)
            .MaxAsync(x => x.SortOrder, ct);
    }

    public async Task<bool> MoveCardAsync(
        Guid taskId, Guid fromColumnId, Guid toColumnId,
        int fromIndex, int toIndex, CancellationToken ct = default)
    {
        var sourceItems = await db.TodoItems
            .Where(x => x.ColumnId == fromColumnId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(ct);

        if (fromIndex < 0 || fromIndex >= sourceItems.Count)
            return false;

        if (sourceItems[fromIndex].Id != taskId)
            return false;

        var movedEntity = sourceItems[fromIndex];
        sourceItems.RemoveAt(fromIndex);

        if (fromColumnId == toColumnId)
        {
            if (toIndex < 0 || toIndex > sourceItems.Count)
                return false;

            sourceItems.Insert(toIndex, movedEntity);

            for (var i = 0; i < sourceItems.Count; i++)
                sourceItems[i].SortOrder = i;
        }
        else
        {
            for (var i = 0; i < sourceItems.Count; i++)
                sourceItems[i].SortOrder = i;

            var targetItems = await db.TodoItems
                .Where(x => x.ColumnId == toColumnId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(ct);

            if (toIndex < 0 || toIndex > targetItems.Count)
                return false;

            movedEntity.ColumnId = toColumnId;
            targetItems.Insert(toIndex, movedEntity);

            for (var i = 0; i < targetItems.Count; i++)
                targetItems[i].SortOrder = i;
        }

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task MoveCardToEndAsync(Guid taskId, Guid targetColumnId, CancellationToken ct = default)
    {
        var entity = await db.TodoItems.FirstOrDefaultAsync(x => x.Id == taskId, ct);
        if (entity is null)
            return;

        var maxSort = await GetMaxSortOrderAsync(targetColumnId, ct);

        entity.ColumnId = targetColumnId;
        entity.SortOrder = maxSort + 1;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
    }
}
