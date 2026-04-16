using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface ITeamCapacityRepository
{
    Task<TeamCapacity?> GetAsync(Guid teamId, CancellationToken ct = default);
    Task<TeamCapacity> CreateAsync(TeamCapacity capacity, CancellationToken ct = default);
    Task<TeamCapacity> UpdateAsync(TeamCapacity capacity, CancellationToken ct = default);
}
