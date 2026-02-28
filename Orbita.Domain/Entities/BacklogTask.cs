using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class BacklogTask
{
    public BacklogId Id { get; private set; }
    public string Title { get; private set; }
    public TodoItemPriority Priority { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public UserId CreatorId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool InWeek { get; private set; }
    public bool IsCompleted { get; private set; }

    public string? DueTime { get; private set; }
    public int? EstimateMinutes { get; private set; }


    private readonly List<UserId> _assignee = [];
    public IReadOnlyCollection<UserId> Assignee => _assignee.AsReadOnly();
}
