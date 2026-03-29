using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Services;

public class TeamService(ITeamRepository teamRepository) : ITeamService
{
    public async Task<Result<Team>> CreateAsync(Guid userId, string name, CancellationToken ct = default)
    {
        var existingTeamId = await teamRepository.GetTeamIdByUserAsync(userId, ct);
        if (existingTeamId is not null)
            return Result<Team>.Fail("User already belongs to a team.");

        var team = Team.Create(name);
        var created = await teamRepository.CreateAsync(team, ct);

        await teamRepository.SetUserTeamAsync(userId, created.Id.Id, ct);
        await teamRepository.SeedDefaultColumnsAsync(created.Id.Id, ct);

        return Result<Team>.Ok(created);
    }

    public async Task<Result> AddMemberAsync(Guid currentUserId, Guid targetUserId, CancellationToken ct = default)
    {
        var teamId = await teamRepository.GetTeamIdByUserAsync(currentUserId, ct);
        if (teamId is null)
            return Result.Fail("You do not belong to any team.");

        var targetTeamId = await teamRepository.GetTeamIdByUserAsync(targetUserId, ct);
        if (targetTeamId is not null)
            return Result.Fail("Target user already belongs to a team.");

        await teamRepository.SetUserTeamAsync(targetUserId, teamId.Value, ct);

        return Result.Ok();
    }
}
