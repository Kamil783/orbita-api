using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities.Mapping;

namespace Orbita.Infrastructure.Entities;

public class UserProfileEntity
{
    public UserId UserId { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public byte[]? AvatarData { get; set; }
    public string? AvatarContentType { get; set; }
    public int AvatarVersion { get; set; } = 1;

    public ICollection<BacklogTaskAssigneeEntity> AssignedBacklogTaskItems { get; set; } = [];
    public UserEntity User { get; set; } = default!;
}
