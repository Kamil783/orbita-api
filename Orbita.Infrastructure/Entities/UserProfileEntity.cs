namespace Orbita.Infrastructure.Entities;

public class UserProfileEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public byte[]? AvatarData { get; set; }
    public string? AvatarContentType { get; set; }
    public int AvatarVersion { get; set; } = 1;

    public UserEntity User { get; set; } = default!;
}
