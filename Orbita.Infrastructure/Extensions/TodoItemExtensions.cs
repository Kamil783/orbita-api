using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class TodoItemExtensions
{
    public static TodoItemEntity ToEntity(this TodoItem item)
    {
        return new TodoItemEntity
        {
            Id = item.Id.Id,
            Title = item.Title,
            TaskStatus = item.TaskStatus,
            TaskPriority = item.TaskPriority,
            CreatorId = item.CreatorId.Id,
            ColumnId = item.ColumnId.Id,
            CreatedAtUtc = item.CreatedAtUtc,
            AssigneeId = item.AssigneeId?.Id,
            UpdatedAtUtc = item.UpdatedAtUtc,
            DeadlineUtc = item.DeadlineUtc,
            ProgressPct = item.ProgressPct,
            BacklogId = item.BacklogId?.Id,
            DeadlineText = item.DeadlineText,
            CompletedText = item.CompletedText,
            SortOrder = item.SortOrder
        };
    }

    public static TodoItem ToDomain(this TodoItemEntity entity)
    {
        return TodoItem.Restore(
            id: new TodoItemId(entity.Id),
            title: entity.Title,
            taskStatus: entity.TaskStatus,
            taskPriority: entity.TaskPriority,
            creatorId: new UserId(entity.CreatorId),
            columnId: new ColumnId(entity.ColumnId),
            createdAtUtc: entity.CreatedAtUtc,
            sortOrder: entity.SortOrder,
            assigneeId: entity.AssigneeId.HasValue ? new UserId(entity.AssigneeId.Value) : null,
            updatedAtUtc: entity.UpdatedAtUtc,
            deadlineUtc: entity.DeadlineUtc,
            progressPct: entity.ProgressPct,
            backlogId: entity.BacklogId.HasValue ? new BacklogTaskId(entity.BacklogId.Value) : null,
            deadlineText: entity.DeadlineText,
            completedText: entity.CompletedText
        );
    }
}
