using Orbita.Domain.Enums;

namespace Orbita.Infrastructure.Entities;

public class CalendarEventEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public CalendarEventType Type { get; set; }

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public CalendarEventColor Color { get; set; }

    public DateOnly Date { get; set; }
    public DateOnly? EndDate { get; set; }

    public string? Location { get; set; }

    public Guid? TaskId { get; set; }
    public TodoItemEntity? Task { get; set; }

    public string? GoogleEventId { get; set; }
}
