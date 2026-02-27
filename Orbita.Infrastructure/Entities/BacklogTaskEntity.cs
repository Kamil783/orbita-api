using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Infrastructure.Entities;

public class BacklogTaskEntity
{
    public BacklogId Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public TodoItemPriority Priority { get; set; }
    public string Description { get; set; } = string.Empty;

    public UserId CreatorId { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public bool InWeek { get; set; }
    public bool IsCompleted { get; set; }

    public UserId? AssigneeId { get; set; }
    public string? DueTime { get; set; }
    public int? EstimateMinutes { get; set; }
}
