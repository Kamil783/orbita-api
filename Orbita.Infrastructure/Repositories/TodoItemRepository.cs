using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Entities.Mapping;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;
using System.Threading.Tasks;

namespace Orbita.Infrastructure.Repositories;

public class TodoItemRepository(OrbitaDbContext db) : ITodoItemRepository
{
    public async Task<TodoItem?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.TodoItems
            .Include(x => x.Assignees)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return entity?.ToDomain();
    }

    public async Task<TodoItem?> GetByBacklogIdAsync(Guid backlogId, CancellationToken ct = default)
    {
        var entity = await db.TodoItems
            .Include(x => x.Assignees)
            .FirstOrDefaultAsync(x => x.BacklogId == backlogId, ct);

        return entity?.ToDomain();
    }

    public async Task<List<TodoItem>> GetByBacklogIdBatchAsync(IEnumerable<Guid> backlogId, CancellationToken ct = default)
    {
        var entity = await db.TodoItems
            .Include(x => x.Assignees)
            .Where(x => x.BacklogId.HasValue && backlogId.Contains(x.BacklogId.Value))
            .ToListAsync(ct);

        return entity.Select(e => e.ToDomain()).ToList();
    }

    public async Task<TodoItem> CreateAsync(TodoItem item, CancellationToken ct = default)
    {
        var entity = item.ToEntity();

        await db.TodoItems.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);

        return entity.ToDomain();
    }

    public async Task<TodoItem?> UpdateAsync(TodoItem item, CancellationToken ct = default)
    {
        var entity = await db.TodoItems
             .Include(x => x.Assignees)
             .FirstOrDefaultAsync(x => x.Id == item.Id.Id, ct);

        if (entity is null)
        {
            return null;
        }

        MapToExistingEntity(item, entity);

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

    public async Task DeleteBatchAsync(IEnumerable<Guid> id, CancellationToken ct = default)
    {
        await db.TodoItems
            .Where(x => id.Contains(x.Id))
            .ExecuteDeleteAsync(ct);
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

    private static void MapToExistingEntity(TodoItem source, TodoItemEntity target)
    {
        target.Title = source.Title;
        target.TaskStatus = source.TaskStatus;
        target.TaskPriority = source.TaskPriority;
        target.CreatorId = source.CreatorId.Id;
        target.TeamId = source.TeamId.Id;
        target.ColumnId = source.ColumnId.Id;
        target.UpdatedAtUtc = source.UpdatedAtUtc;
        target.DeadlineUtc = source.DeadlineUtc;
        target.ProgressPct = source.ProgressPct;
        target.BacklogId = source.BacklogId?.Id;
        target.CompletedText = source.CompletedText;
        target.SortOrder = source.SortOrder;

        target.Assignees.Clear();

        foreach(var item in source.Assignees)
        {
            target.Assignees.Add(new TodoItemAssigneeEntity
            {
                TodoItemId = target.Id,
                UserId = item.Id
            });
        }
    }
}
