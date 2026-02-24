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
    public BacklogId? BacklogId { get; private set; } 


    public string? DeadlineText { get; private set; }
    public string? CompletedText { get; private set; }

}
