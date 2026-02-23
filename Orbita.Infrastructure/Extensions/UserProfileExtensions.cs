using Orbita.Domain.Entities;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class UserProfileExtensions
{
    public static UserProfileEntity ToEntity(this UserProfile profile)
    {
        return new UserProfileEntity
        {
            UserId = profile.UserId,
            Name = profile.Name,
            AvatarData = profile.AvatarData,
            AvatarContentType = profile.AvatarContentType,
            AvatarVersion = profile.AvatarVersion
        };
    }

    public static UserProfile ToDomain(this UserProfileEntity userEntity)
    {
        return new UserProfile
        {
            UserId = userEntity.UserId,
            Name = userEntity.Name,
            AvatarData = userEntity.AvatarData,
            AvatarContentType = userEntity.AvatarContentType,
            AvatarVersion = userEntity.AvatarVersion
        };
    }
}
