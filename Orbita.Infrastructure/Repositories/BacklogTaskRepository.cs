using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Entities.Mapping;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class BacklogTaskRepository(OrbitaDbContext db) : IBacklogTaskRepository
{
    public async Task<BacklogTask> CreateAsync(BacklogTask task, CancellationToken ct = default)
    {
        var entity = task.ToEntity();

        await db.BacklogTasks.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);

        return entity.ToDomain();
    }

    public async Task<BacklogTask?> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.BacklogTasks
            .Include(x => x.Assignees)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity is null)
        {
            return null;
        }

        db.BacklogTasks.Remove(entity);
        await db.SaveChangesAsync(ct);

        return entity.ToDomain();
    }

    public async Task<BacklogTask?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.BacklogTasks
            .Include(x => x.Assignees)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyCollection<BacklogTask>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await db.BacklogTasks
            .Include(x => x.Assignees)
            .ToListAsync(ct);

        return entities
            .Select(x => x.ToDomain())
            .ToList();
    }

    public async Task<IReadOnlyCollection<BacklogTask>> GetByTeamAsync(Guid teamId, CancellationToken ct = default)
    {
        var entities = await db.BacklogTasks
            .Include(x => x.Assignees)
            .Where(x => x.TeamId == teamId)
            .ToListAsync(ct);

        return entities
            .Select(x => x.ToDomain())
            .ToList();
    }

    public async Task<BacklogTask?> UpdateAsync(BacklogTask task, CancellationToken ct = default)
    {
        var entity = await db.BacklogTasks
            .Include(x => x.Assignees)
            .FirstOrDefaultAsync(x => x.Id == task.Id.Id, ct);

        if (entity is null)
        {
            return null;
        }

        MapToExistingEntity(task, entity);

        await db.SaveChangesAsync(ct);

        return entity.ToDomain();
    }

    private static void MapToExistingEntity(BacklogTask source, BacklogTaskEntity target)
    {
        target.Title = source.Title;
        target.Priority = source.Priority;
        target.Description = source.Description;
        target.TeamId = source.TeamId.Id;
        target.InWeek = source.InWeek;
        target.IsCompleted = source.IsCompleted;
        target.DueDate = source.DueDate;
        target.EstimateMinutes = source.EstimateMinutes;
        target.ProgressPct = source.ProgressPct;

        target.Assignees.Clear();

        foreach (var assignee in source.Assignees)
        {
            target.Assignees.Add(new BacklogTaskAssigneeEntity
            {
                BacklogTaskId = target.Id,
                UserId = assignee.Id
            });
        }
    }
}