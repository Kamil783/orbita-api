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
    public TeamId TeamId { get; private set; }
    public ColumnId ColumnId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? DeadlineUtc { get; private set; }
    public int? ProgressPct { get; private set; }
    public BacklogTaskId? BacklogId { get; private set; }

    public string? CompletedText { get; private set; }
    public int SortOrder { get; private set; }

    private readonly List<UserId> _assignees = [];
    public IReadOnlyCollection<UserId> Assignees => _assignees.AsReadOnly();

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
        string? completedText = null,
        IEnumerable<UserId>? assignees = null)
    {
        var item = new TodoItem
        {
            Id = new TodoItemId(Guid.NewGuid()),
            Title = title,
            TaskStatus = TodoItemStatus.Todo,
            TaskPriority = priority,
            CreatorId = creatorId,
            ColumnId = columnId,
            CreatedAtUtc = DateTime.UtcNow,
            SortOrder = sortOrder,
            DeadlineUtc = NormalizeToUtc(deadlineUtc),
            ProgressPct = progressPct,
            BacklogId = backlogId,
            CompletedText = completedText
        };
        if (assignees is not null)
            item._assignees.AddRange(assignees);
        return item;
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
        IEnumerable<UserId>? assignees = null,
        DateTime? updatedAtUtc = null,
        DateTime? deadlineUtc = null,
        int? progressPct = null,
        BacklogTaskId? backlogId = null,
        string? completedText = null)
    {
        var item = new TodoItem
        {
            Id = id,
            Title = title,
            TaskStatus = taskStatus,
            TaskPriority = taskPriority,
            CreatorId = creatorId,
            ColumnId = columnId,
            CreatedAtUtc = createdAtUtc,
            SortOrder = sortOrder,
            UpdatedAtUtc = updatedAtUtc,
            DeadlineUtc = NormalizeToUtc(deadlineUtc),
            ProgressPct = progressPct,
            BacklogId = backlogId,
            CompletedText = completedText
        };
        if (assignees is not null)
            item._assignees.AddRange(assignees);
        return item;
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

    public void SyncFromBacklog(BacklogTask backlogTask)
    {
        Title = backlogTask.Title;
        TaskPriority = backlogTask.Priority;
        DeadlineUtc = NormalizeToUtc(backlogTask.DueDate);
        ProgressPct = backlogTask.TrackProgress ? backlogTask.ProgressPct : null;

        _assignees.Clear();
        _assignees.AddRange(backlogTask.Assignees);

        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static DateTime? NormalizeToUtc(DateTime? dt)
    {
        if (dt is null) return null;
        return dt.Value.Kind == DateTimeKind.Utc
            ? dt
            : DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
    }
}
