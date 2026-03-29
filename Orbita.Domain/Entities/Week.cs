using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class Week
{
    public WeekId Id { get; private set; }
    public UserId CreatorId { get; private set; }
    public TeamId TeamId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsArchived { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<BacklogTaskId> _taskIds = [];
    public IReadOnlyCollection<BacklogTaskId> TaskIds => _taskIds.AsReadOnly();

    private Week() { }

    public static Week Create(
        UserId creatorId,
        TeamId teamId,
        DateTime startDate,
        DateTime endDate)
    {
        if (endDate <= startDate)
            throw new ArgumentException("EndDate must be after StartDate.", nameof(endDate));

        return new Week
        {
            Id = new WeekId(Guid.NewGuid()),
            CreatorId = creatorId,
            TeamId = teamId,
            StartDate = NormalizeToUtc(startDate),
            EndDate = NormalizeToUtc(endDate),
            IsArchived = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Week Restore(
        WeekId id,
        UserId creatorId,
        TeamId teamId,
        DateTime startDate,
        DateTime endDate,
        bool isArchived,
        DateTime createdAt,
        IEnumerable<BacklogTaskId>? taskIds = null)
    {
        var week = new Week
        {
            Id = id,
            CreatorId = creatorId,
            TeamId = teamId,
            StartDate = startDate,
            EndDate = endDate,
            IsArchived = isArchived,
            CreatedAt = createdAt
        };
        if (taskIds is not null)
            week._taskIds.AddRange(taskIds);
        return week;
    }

    public void Archive()
    {
        IsArchived = true;
    }

    public void AddTask(BacklogTaskId taskId)
    {
        if (!_taskIds.Contains(taskId))
            _taskIds.Add(taskId);
    }

    public void RemoveTask(BacklogTaskId taskId)
    {
        _taskIds.Remove(taskId);
    }

    private static DateTime NormalizeToUtc(DateTime dt)
    {
        return dt.Kind == DateTimeKind.Utc
            ? dt
            : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    }
}
