using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class CalendarEvent
{
    public CalendarEventId Id { get; private set; }
    public string Title { get; private set; }
    public CalendarEventType Type { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public CalendarEventColor Color { get; private set; }
    public DateOnly Date { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public string? Location { get; private set; }
    public TodoItemId? TaskId { get; private set; }
    public string? GoogleEventId { get; private set; }
}
