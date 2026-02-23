using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid userId, CancellationToken ct = default);

    Task<UserProfile?> Update(UserProfile profile, CancellationToken ct = default);
}
