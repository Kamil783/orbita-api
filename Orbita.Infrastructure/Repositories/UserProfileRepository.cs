using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class UserProfileRepository(OrbitaDbContext dbContext) : IUserProfileRepository
{
    public async Task<UserProfile?> GetByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var dbEntity =  await dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(up => up.UserId == userId, ct);

        return dbEntity?.ToDomain();
    }

    public async Task<UserProfile?> Update(UserProfile userProfile, CancellationToken ct = default)
    {
        var updated = await dbContext.UserProfiles
        .Where(x => x.UserId == userProfile.UserId)
        .ExecuteUpdateAsync(s => s
            .SetProperty(x => x.Name, userProfile.Name)
            .SetProperty(x => x.AvatarData, userProfile.AvatarData)
            .SetProperty(x => x.AvatarContentType, userProfile.AvatarContentType)
            .SetProperty(x => x.AvatarVersion, userProfile.AvatarVersion),
            ct);

        if (updated == 0)
        {
            return null;
        }

        await dbContext.SaveChangesAsync(ct);
        return userProfile;
    }
}
