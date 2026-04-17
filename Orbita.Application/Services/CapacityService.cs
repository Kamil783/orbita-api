using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;

namespace Orbita.Application.Services;

public class CapacityService(
    ITeamCapacityRepository teamCapacityRepository,
    ITeamProvider teamProvider) : ICapacityService
{
    public async Task<Result<TeamCapacity>> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var capacity = await teamCapacityRepository.GetAsync(teamId, ct);
        if (capacity is null)
        {
            capacity = TeamCapacity.Create(new TeamId(teamId));
            capacity = await teamCapacityRepository.CreateAsync(capacity, ct);
        }

        return Result<TeamCapacity>.Ok(capacity);
    }

    public async Task<Result<TeamCapacity>> UpdateAsync(
        Guid userId, int weekdayHours, int weekendHours, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var capacity = await teamCapacityRepository.GetAsync(teamId, ct);
        if (capacity is null)
        {
            capacity = TeamCapacity.Create(new TeamId(teamId));
            await teamCapacityRepository.CreateAsync(capacity, ct);
        }

        capacity.Update(weekdayHours, weekendHours);
        var updated = await teamCapacityRepository.UpdateAsync(capacity, ct);

        return Result<TeamCapacity>.Ok(updated);
    }
}
