using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Infrastructure.Entities;

public class TodoItemEntity
{
    public TodoItemId Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public TodoItemStatus TaskStatus { get; set; }
    public TodoItemPriority TaskPriority { get; set; }

    public UserId CreatorId { get; set; } = default!;
    public ColumnId ColumnId { get; set; } = default!;
    public ColumnEntity Column { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; }

    public UserId? AssigneeId { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? DeadlineUtc { get; set; }
    public int? ProgressPct { get; set; }

    public BacklogId? BacklogId { get; set; }

    public string? DeadlineText { get; set; }
    public string? CompletedText { get; set; }

    public CalendarEventEntity? CalendarEvent { get; set; }
}
