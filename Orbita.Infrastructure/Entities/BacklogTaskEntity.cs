using Orbita.Domain.Enums;
using Orbita.Infrastructure.Entities.Mapping;

namespace Orbita.Infrastructure.Entities;

public class BacklogTaskEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public TodoItemPriority Priority { get; set; }
    public string Description { get; set; } = string.Empty;

    public Guid CreatorId { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool InWeek { get; set; }
    public bool IsCompleted { get; set; }

    public ICollection<BacklogTaskAssigneeEntity> Assignees { get; set; } = [];
    public string? DueTime { get; set; }
    public int? EstimateMinutes { get; set; }
}
