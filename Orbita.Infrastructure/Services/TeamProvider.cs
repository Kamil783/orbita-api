using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Domain.Entities;

namespace Orbita.Infrastructure.Services;

public class TeamProvider(ITeamRepository teamRepository) : ITeamProvider
{
    public async Task<Guid> GetTeamIdAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamRepository.GetTeamIdByUserAsync(userId, ct);

        if (teamId is not null)
            return teamId.Value;

        var team = Team.Create($"Personal");
        await teamRepository.CreateAsync(team, ct);
        await teamRepository.SetUserTeamAsync(userId, team.Id.Id, ct);
        await teamRepository.SeedDefaultColumnsAsync(team.Id.Id, ct);

        return team.Id.Id;
    }
}
