using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class RecurringTask
{
    public RecurringTaskId Id { get; private set; }
    public UserId CreatorId { get; private set; }
    public TeamId TeamId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public int DayOfMonth { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? LastResetAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private RecurringTask() { }

    public static RecurringTask Create(
        UserId creatorId,
        TeamId teamId,
        string title,
        string? description,
        int dayOfMonth)
    {
        ValidateTitle(title);
        ValidateDayOfMonth(dayOfMonth);

        var now = DateTime.UtcNow;
        return new RecurringTask
        {
            Id = new RecurringTaskId(Guid.NewGuid()),
            CreatorId = creatorId,
            TeamId = teamId,
            Title = title.Trim(),
            Description = NormalizeDescription(description),
            DayOfMonth = dayOfMonth,
            IsCompleted = false,
            LastResetAt = null,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public static RecurringTask Restore(
        RecurringTaskId id,
        UserId creatorId,
        TeamId teamId,
        string title,
        string? description,
        int dayOfMonth,
        bool isCompleted,
        DateTime? lastResetAt,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new RecurringTask
        {
            Id = id,
            CreatorId = creatorId,
            TeamId = teamId,
            Title = title,
            Description = description,
            DayOfMonth = dayOfMonth,
            IsCompleted = isCompleted,
            LastResetAt = lastResetAt,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public void Update(
        string? title,
        string? description,
        bool clearDescription,
        int? dayOfMonth,
        bool? isCompleted)
    {
        if (title is not null)
        {
            ValidateTitle(title);
            Title = title.Trim();
        }

        if (clearDescription)
            Description = null;
        else if (description is not null)
            Description = NormalizeDescription(description);

        if (dayOfMonth.HasValue)
        {
            ValidateDayOfMonth(dayOfMonth.Value);
            DayOfMonth = dayOfMonth.Value;
        }

        if (isCompleted.HasValue)
            IsCompleted = isCompleted.Value;

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkReset(DateTime utcNow)
    {
        IsCompleted = false;
        LastResetAt = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (title.Trim().Length > 200)
            throw new ArgumentException("Title is too long (max 200).", nameof(title));
    }

    private static void ValidateDayOfMonth(int dayOfMonth)
    {
        if (dayOfMonth is < 1 or > 31)
            throw new ArgumentOutOfRangeException(nameof(dayOfMonth), "DayOfMonth must be between 1 and 31.");
    }

    private static string? NormalizeDescription(string? description)
    {
        if (description is null) return null;
        var trimmed = description.Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }
}
