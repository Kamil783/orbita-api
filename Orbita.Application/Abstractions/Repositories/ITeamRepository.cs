using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface ITeamRepository
{
    Task<Team?> GetAsync(Guid teamId, CancellationToken ct = default);
    Task<List<Team>> GetAllAsync(CancellationToken ct = default);
    Task<Team> CreateAsync(Team team, CancellationToken ct = default);
    Task<Guid?> GetTeamIdByUserAsync(Guid userId, CancellationToken ct = default);
    Task SetUserTeamAsync(Guid userId, Guid teamId, CancellationToken ct = default);
}
