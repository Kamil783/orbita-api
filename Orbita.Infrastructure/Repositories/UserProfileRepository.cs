using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class UserProfileRepository(OrbitaDbContext db) : IUserProfileRepository
{
    public async Task<UserProfile?> GetByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var dbEntity =  await db.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(up => up.UserId == userId, ct);

        return dbEntity?.ToDomain();
    }

    public async Task<UserProfile?> Update(UserProfile userProfile, CancellationToken ct = default)
    {
        var entity = await db.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == userProfile.UserId.Id, ct);

        if (entity is null)
        {
            return null;
        }

        MapToExistingEntity(userProfile, entity);

        await db.SaveChangesAsync(ct);

        return entity.ToDomain();
    }

    private static void MapToExistingEntity(UserProfile source, UserProfileEntity target)
    {
        target.Name = source.Name;
        target.AvatarData = source.AvatarData;
        target.AvatarContentType = source.AvatarContentType;
        target.AvatarVersion = source.AvatarVersion;
    }
}
