using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class TeamRepository(OrbitaDbContext db) : ITeamRepository
{
    public async Task<Team?> GetAsync(Guid teamId, CancellationToken ct = default)
    {
        var entity = await db.Teams
            .Include(t => t.TeamMembers)
            .FirstOrDefaultAsync(t => t.Id == teamId, ct);

        if (entity is null)
            return null;

        var members = entity.TeamMembers.Select(m => new User
        {
            Id = m.Id,
            FullName = "",
            Email = m.Email ?? ""
        });

        return Team.Restore(entity.Id, entity.Name, entity.CreatedAt, entity.UpdatedAt ?? entity.CreatedAt, members);
    }

    public async Task<Team> CreateAsync(Team team, CancellationToken ct = default)
    {
        var entity = new TeamEntity
        {
            Id = team.Id.Id,
            Name = team.Name,
            CreatedAt = team.CreatedAt,
            UpdatedAt = team.UpdatedAt
        };

        await db.Teams.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);

        return team;
    }

    public async Task<Guid?> GetTeamIdByUserAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        return user?.TeamId;
    }

    public async Task SetUserTeamAsync(Guid userId, Guid teamId, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is not null)
        {
            user.TeamId = teamId;
            await db.SaveChangesAsync(ct);
        }
    }
}
