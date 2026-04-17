using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class BacklogTask
{
    public BacklogTaskId Id { get; private set; }
    public string Title { get; private set; }
    public TodoItemPriority Priority { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public UserId CreatorId { get; private set; }
    public TeamId TeamId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool InWeek { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsArchived { get; private set; }

    public DateTime? DueDate { get; private set; }
    public int? EstimateMinutes { get; private set; }
    public int? ProgressPct { get; private set; }

    private readonly List<UserId> _assignees = [];
    public IReadOnlyCollection<UserId> Assignees => _assignees.AsReadOnly();

    //private readonly List<WeekId> _weeks = new List<WeekId>();
    //public IReadOnlyCollection<WeekId> Weeks => _weeks.AsReadOnly();

    private readonly List<TimeEntry> _timeEntries = [];
    public IReadOnlyCollection<TimeEntry> TimeEntries => _timeEntries.AsReadOnly();
    public int LoggedMinutes => _timeEntries.Sum(e => e.Minutes);

    public bool TrackProgress => ProgressPct is not null;

    private BacklogTask(
        BacklogTaskId id,
        string title,
        TodoItemPriority priority,
        string description,
        UserId creatorId,
        TeamId teamId,
        DateTime createdAt,
        bool inWeek,
        bool isCompleted,
        bool isArchived,
        DateTime? dueDate,
        int? estimateMinutes,
        int? progressPct,
        IEnumerable<UserId> assignees,
        IEnumerable<TimeEntry>? timeEntries = null)
    {
        Id = id;
        Title = title;
        Priority = priority;
        Description = description;
        CreatorId = creatorId;
        TeamId = teamId;
        CreatedAt = createdAt;
        InWeek = inWeek;
        IsCompleted = isCompleted;
        IsArchived = isArchived;
        DueDate = NormalizeToUtc(dueDate);
        EstimateMinutes = estimateMinutes;
        ProgressPct = progressPct;
        _assignees = [.. assignees];
        _timeEntries = timeEntries is not null ? [.. timeEntries] : [];
    }

    public static BacklogTask Restore(
        BacklogTaskId id,
        string title,
        TodoItemPriority priority,
        string description,
        UserId creatorId,
        TeamId teamId,
        DateTime createdAt,
        bool inWeek,
        bool isCompleted,
        bool isArchived,
        DateTime? dueDate,
        int? estimateMinutes,
        int? progressPct,
        IEnumerable<UserId> assignees,
        IEnumerable<TimeEntry>? timeEntries = null)
    {
        return new BacklogTask(
            id,
            title,
            priority,
            description,
            creatorId,
            teamId,
            createdAt,
            inWeek,
            isCompleted,
            isArchived,
            dueDate,
            estimateMinutes,
            progressPct,
            assignees,
            timeEntries);
    }

    public static BacklogTask Create(
        string title,
        TodoItemPriority priority,
        string description,
        UserId creatorId,
        TeamId teamId,
        DateTime? dueDate,
        int? estimateMinutes,
        int? progressPct,
        IEnumerable<UserId> assignees)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (estimateMinutes is < 0)
            throw new ArgumentOutOfRangeException(nameof(estimateMinutes), "Estimate cannot be negative.");

        return new BacklogTask(
            new BacklogTaskId(Guid.NewGuid()),
            title,
            priority,
            description,
            creatorId,
            teamId,
            DateTime.UtcNow,
            false,
            false,
            false,
            dueDate,
            estimateMinutes,
            progressPct,
            assignees);
    }

    public void SetDueDate(DateTime? dueDate)
    {
        DueDate = NormalizeToUtc(dueDate);
    }

    public void SetEstimateMinutes(int? estimateMinutes)
    {
        if (estimateMinutes is < 0)
            throw new ArgumentOutOfRangeException(nameof(estimateMinutes), "Estimate cannot be negative.");

        EstimateMinutes = estimateMinutes;
    }

    public bool HasDeadline() => DueDate.HasValue;

    public bool IsOverdue(DateTime now)
    {
        if (!DueDate.HasValue || IsCompleted)
            return false;

        return DueDate.Value.Date < now.Date;
    }

    public bool IsDueToday(DateTime now)
    {
        if (!DueDate.HasValue)
            return false;

        return DueDate.Value.Date == now.Date;
    }

    public bool IsDueTomorrow(DateTime now)
    {
        if (!DueDate.HasValue)
            return false;

        return DueDate.Value.Date == now.Date.AddDays(1);
    }

    public void SetInWeek(bool inWeek)
    {
        InWeek = inWeek;
    }

    public void SetCompleted(bool completed)
    {
        IsCompleted = completed;
    }

    public void SetArchived(bool archived)
    {
        IsArchived = archived;
    }

    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        Title = title;
    }

    public void SetDescription(string description)
    {
        Description = description;
    }

    public void SetPriority(TodoItemPriority priority)
    {
        Priority = priority;
    }

    public void SetProgressPct(int? progressPct)
    {
        ProgressPct = progressPct;
    }

    public void SetAssignees(IEnumerable<UserId> assignees)
    {
        _assignees.Clear();
        _assignees.AddRange(assignees);
    }

    private static DateTime? NormalizeToUtc(DateTime? dt)
    {
        if (dt is null) return null;
        return dt.Value.Kind == DateTimeKind.Utc
            ? dt
            : DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
    }
}
