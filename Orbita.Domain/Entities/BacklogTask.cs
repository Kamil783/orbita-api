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
    public DateTime CreatedAt { get; private set; }
    public bool InWeek { get; private set; }
    public bool IsCompleted { get; private set; }

    public string? DueTime { get; private set; }
    public int? EstimateMinutes { get; private set; }


    private readonly List<UserId> _assignees = [];
    public IReadOnlyCollection<UserId> Assignees => _assignees.AsReadOnly();

    private BacklogTask(
        BacklogTaskId id,
        string title,
        TodoItemPriority priority,
        string description,
        UserId creatorId,
        DateTime createdAt,
        bool inWeek,
        bool isCompleted,
        string? dueTime,
        int? estimateMinutes,
        IEnumerable<UserId> assignees)
    {
        Id = id;
        Title = title;
        Priority = priority;
        Description = description;
        CreatorId = creatorId;
        CreatedAt = createdAt;
        InWeek = inWeek;
        IsCompleted = isCompleted;
        DueTime = dueTime;
        EstimateMinutes = estimateMinutes;
        _assignees = [.. assignees];
    }

    public static BacklogTask Restore(
        BacklogTaskId id,
        string title,
        TodoItemPriority priority,
        string description,
        UserId creatorId,
        DateTime createdAt,
        bool inWeek,
        bool isCompleted,
        string? dueTime,
        int? estimateMinutes,
        IEnumerable<UserId> assignees)
    {
        return new BacklogTask(
            id,
            title,
            priority,
            description,
            creatorId,
            createdAt,
            inWeek,
            isCompleted,
            dueTime,
            estimateMinutes,
            assignees);
    }
}
