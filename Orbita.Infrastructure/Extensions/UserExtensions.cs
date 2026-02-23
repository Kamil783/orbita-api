using Orbita.Domain.Entities;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class UserExtensions
{
    public static UserEntity ToEntity(this User user)
    {
        return new UserEntity
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.Email
        };
    }

    public static User ToDomain(this UserEntity userEntity)
    {
        return new User
        {
            Id = userEntity.Id,
            Email = userEntity.Email
        };
    }
}
