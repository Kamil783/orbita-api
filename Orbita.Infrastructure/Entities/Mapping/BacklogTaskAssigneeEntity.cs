namespace Orbita.Infrastructure.Entities.Mapping;

public class BacklogTaskAssigneeEntity
{
    public Guid BacklogTaskId { get; set; }
    public BacklogTaskEntity BacklogTask { get; set; } = default!;

    public Guid UserId { get; set; }
    public UserProfileEntity UserProfile { get; set; } = default!;
}
