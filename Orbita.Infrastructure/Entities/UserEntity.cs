using Microsoft.AspNetCore.Identity;

namespace Orbita.Infrastructure.Entities;

public class UserEntity : IdentityUser<Guid>
{
    public UserProfileEntity? UserProfile { get; set; }
}
