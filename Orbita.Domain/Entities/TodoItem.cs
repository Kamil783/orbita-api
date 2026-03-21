using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class TodoItem
{
    public TodoItemId Id { get; private set; }
    public string Title { get; private set; }
    public TodoItemStatus TaskStatus { get; private set; }
    public TodoItemPriority TaskPriority { get; private set; }
    public UserId CreatorId { get; private set; }
    public ColumnId ColumnId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public UserId? AssigneeId { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? DeadlineUtc { get; private set; }
    public int? ProgressPct { get; private set; }
    public BacklogTaskId? BacklogId { get; private set; }

    public string? DeadlineText { get; private set; }
    public string? CompletedText { get; private set; }
    public int SortOrder { get; private set; }

    private TodoItem() { }

    public static TodoItem Create(
        string title,
        TodoItemPriority priority,
        UserId creatorId,
        ColumnId columnId,
        int sortOrder,
        DateTime? deadlineUtc = null,
        int? progressPct = null,
        BacklogTaskId? backlogId = null,
        string? deadlineText = null,
        string? completedText = null,
        UserId? assigneeId = null)
    {
        return new TodoItem
        {
            Id = new TodoItemId(Guid.NewGuid()),
            Title = title,
            TaskStatus = TodoItemStatus.Todo,
            TaskPriority = priority,
            CreatorId = creatorId,
            ColumnId = columnId,
            CreatedAtUtc = DateTime.UtcNow,
            SortOrder = sortOrder,
            DeadlineUtc = deadlineUtc,
            ProgressPct = progressPct,
            BacklogId = backlogId,
            DeadlineText = deadlineText,
            CompletedText = completedText,
            AssigneeId = assigneeId
        };
    }

    public static TodoItem Restore(
        TodoItemId id,
        string title,
        TodoItemStatus taskStatus,
        TodoItemPriority taskPriority,
        UserId creatorId,
        ColumnId columnId,
        DateTime createdAtUtc,
        int sortOrder,
        UserId? assigneeId = null,
        DateTime? updatedAtUtc = null,
        DateTime? deadlineUtc = null,
        int? progressPct = null,
        BacklogTaskId? backlogId = null,
        string? deadlineText = null,
        string? completedText = null)
    {
        return new TodoItem
        {
            Id = id,
            Title = title,
            TaskStatus = taskStatus,
            TaskPriority = taskPriority,
            CreatorId = creatorId,
            ColumnId = columnId,
            CreatedAtUtc = createdAtUtc,
            SortOrder = sortOrder,
            AssigneeId = assigneeId,
            UpdatedAtUtc = updatedAtUtc,
            DeadlineUtc = deadlineUtc,
            ProgressPct = progressPct,
            BacklogId = backlogId,
            DeadlineText = deadlineText,
            CompletedText = completedText
        };
    }

    public void MoveTo(ColumnId columnId, int sortOrder)
    {
        ColumnId = columnId;
        SortOrder = sortOrder;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }
}
