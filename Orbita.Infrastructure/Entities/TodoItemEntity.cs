using Orbita.Domain.Enums;
using Orbita.Infrastructure.Entities.Mapping;

namespace Orbita.Infrastructure.Entities;

public class TodoItemEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public TodoItemStatus TaskStatus { get; set; }
    public TodoItemPriority TaskPriority { get; set; }

    public Guid CreatorId { get; set; }
    public Guid ColumnId { get; set; }
    public ColumnEntity Column { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? DeadlineUtc { get; set; }
    public int? ProgressPct { get; set; }

    public Guid? BacklogId { get; set; }

    public string? CompletedText { get; set; }
    public int SortOrder { get; set; }

    public ICollection<TodoItemAssigneeEntity> Assignees { get; set; } = [];
    public CalendarEventEntity? CalendarEvent { get; set; }
}
