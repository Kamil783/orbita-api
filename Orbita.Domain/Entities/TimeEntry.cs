using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class TimeEntry
{
    public TimeEntryId Id { get; private set; }
    public BacklogTaskId BacklogTaskId { get; private set; }
    public UserId UserId { get; private set; }
    public int Minutes { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private TimeEntry() { }

    public static TimeEntry Create(
        BacklogTaskId backlogTaskId,
        UserId userId,
        int minutes,
        string? description)
    {
        if (minutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(minutes), "Minutes must be positive.");

        return new TimeEntry
        {
            Id = new TimeEntryId(Guid.NewGuid()),
            BacklogTaskId = backlogTaskId,
            UserId = userId,
            Minutes = minutes,
            Description = string.IsNullOrWhiteSpace(description) ? null : description,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static TimeEntry Restore(
        TimeEntryId id,
        BacklogTaskId backlogTaskId,
        UserId userId,
        int minutes,
        string? description,
        DateTime createdAt)
    {
        return new TimeEntry
        {
            Id = id,
            BacklogTaskId = backlogTaskId,
            UserId = userId,
            Minutes = minutes,
            Description = description,
            CreatedAt = createdAt
        };
    }
}
