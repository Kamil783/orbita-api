using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Entities.Mapping;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class WeekRepository(OrbitaDbContext db) : IWeekRepository
{
    public async Task<Week?> GetCurrentAsync(Guid userId, CancellationToken ct = default)
    {
        var entity = await db.Weeks
            .Include(w => w.BacklogTaskWeeks)
            .Where(w => w.CreatorId == userId && !w.IsArchived)
            .OrderByDescending(w => w.CreatedAt)
            .FirstOrDefaultAsync(ct);

        return entity?.ToDomain();
    }

    public async Task<List<Week>> GetArchivedAsync(Guid userId, CancellationToken ct = default)
    {
        var entities = await db.Weeks
            .Include(w => w.BacklogTaskWeeks)
            .Where(w => w.CreatorId == userId && w.IsArchived)
            .OrderByDescending(w => w.StartDate)
            .ToListAsync(ct);

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<Week> CreateAsync(Week week, CancellationToken ct = default)
    {
        var entity = week.ToEntity();

        await db.Weeks.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);

        return entity.ToDomain();
    }

    public async Task<Week?> UpdateAsync(Week week, CancellationToken ct = default)
    {
        var entity = await db.Weeks
            .Include(w => w.BacklogTaskWeeks)
            .FirstOrDefaultAsync(w => w.Id == week.Id.Id, ct);

        if (entity is null)
            return null;

        entity.StartDate = week.StartDate;
        entity.EndDate = week.EndDate;
        entity.IsArchived = week.IsArchived;

        entity.BacklogTaskWeeks.Clear();
        foreach (var taskId in week.TaskIds)
        {
            entity.BacklogTaskWeeks.Add(new BacklogTaskWeekEntity
            {
                BacklogTaskId = taskId.Id,
                WeekId = entity.Id
            });
        }

        await db.SaveChangesAsync(ct);

        return entity.ToDomain();
    }

    public async Task<List<Week>> GetByBacklogTaskAsync(Guid backlogTaskId, CancellationToken ct = default)
    {
        var entities = await db.Weeks
            .Include(w => w.BacklogTaskWeeks)
            .Where(w => w.BacklogTaskWeeks.Any(btw => btw.BacklogTaskId == backlogTaskId))
            .OrderBy(w => w.StartDate)
            .ToListAsync(ct);

        return entities.Select(e => e.ToDomain()).ToList();
    }
}
