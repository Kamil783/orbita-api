using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class TeamCapacityRepository(OrbitaDbContext db) : ITeamCapacityRepository
{
    public async Task<TeamCapacity?> GetAsync(Guid teamId, CancellationToken ct = default)
    {
        var entity = await db.TeamCapacities
            .FirstOrDefaultAsync(x => x.TeamId == teamId, ct);

        return entity?.ToDomain();
    }

    public async Task<TeamCapacity> CreateAsync(TeamCapacity capacity, CancellationToken ct = default)
    {
        var entity = capacity.ToEntity();
        await db.TeamCapacities.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task<TeamCapacity> UpdateAsync(TeamCapacity capacity, CancellationToken ct = default)
    {
        var entity = await db.TeamCapacities
            .FirstOrDefaultAsync(x => x.TeamId == capacity.TeamId.Id, ct);

        if (entity is null)
        {
            entity = capacity.ToEntity();
            await db.TeamCapacities.AddAsync(entity, ct);
        }
        else
        {
            entity.WeekdayHours = capacity.WeekdayHours;
            entity.WeekendHours = capacity.WeekendHours;
        }

        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }
}
