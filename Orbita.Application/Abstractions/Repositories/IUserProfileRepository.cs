using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid userId, CancellationToken ct = default);

    Task<UserProfile?> UpdateAsync(UserProfile profile, CancellationToken ct = default);
    Task<IEnumerable<UserProfile>> GetTeamUserProfilesAsync(Guid userId, CancellationToken ct = default);
}
