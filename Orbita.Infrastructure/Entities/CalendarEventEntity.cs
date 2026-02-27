using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Infrastructure.Entities;

public class CalendarEventEntity
{
    public CalendarEventId Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public CalendarEventType Type { get; set; }

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public CalendarEventColor Color { get; set; }

    public DateOnly Date { get; set; }
    public DateOnly? EndDate { get; set; }

    public string? Location { get; set; }

    public TodoItemId? TaskId { get; set; }
    public TodoItemEntity? Task { get; set; }

    public string? GoogleEventId { get; set; }
}