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

        return Result<Team>.Ok(created);
    }

    public async Task<Result<List<Team>>> GetAsync(CancellationToken ct = default)
    {
        var teams = await teamRepository.GetAllAsync(ct);

        return Result<List<Team>>.Ok(teams);
    }

    public async Task<Result> AddMemberAsync(Guid teamId, Guid targetUserId, CancellationToken ct = default)
    {
        var targetTeamId = await teamRepository.GetTeamIdByUserAsync(targetUserId, ct);
        if (targetTeamId is not null)
            return Result.Fail("Target user already belongs to a team.");

        await teamRepository.SetUserTeamAsync(targetUserId, teamId, ct);

        return Result.Ok();
    }
}
