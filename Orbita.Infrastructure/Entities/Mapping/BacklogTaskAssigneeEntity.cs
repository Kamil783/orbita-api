using Orbita.Domain.ValueObjects;

namespace Orbita.Infrastructure.Entities.Mapping;

public class BacklogTaskAssigneeEntity
{
    public BacklogId BacklogTaskId { get; set; } = default!;
    public BacklogTaskEntity BacklogTask { get; set; } = default!;

    public UserId UserId { get; set; } = default!;
    public UserProfileEntity UserProfile { get; set; } = default!;
}
