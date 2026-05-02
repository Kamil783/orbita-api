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
            .Include(x => x.TimeEntries)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity is null)
        {
            return null;
        }

        db.BacklogTasks.Remove(entity);
        await db.SaveChangesAsync(ct);

        return entity.ToDomain();
    }

    public async Task DeleteBatchAsync(IEnumerable<Guid> id, CancellationToken ct = default)
    {
        await db.BacklogTasks
            .Where(x => id.Contains(x.Id))
            .ExecuteDeleteAsync(ct);
    }

    public async Task ArchiveBatchAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        await db.BacklogTasks
            .Where(x => idList.Contains(x.Id))
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.IsArchived, true)
                .SetProperty(x => x.InWeek, false),
                ct);
    }

    public async Task<BacklogTask?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.BacklogTasks
            .Include(x => x.Assignees)
            .Include(x => x.TimeEntries)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyCollection<BacklogTask>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await db.BacklogTasks
            .Include(x => x.Assignees)
            .Include(x => x.TimeEntries)
            .ToListAsync(ct);

        return entities
            .Select(x => x.ToDomain())
            .ToList();
    }

    public async Task<IReadOnlyCollection<BacklogTask>> GetAllActiveAsync(CancellationToken ct = default)
    {
        var entities = await db.BacklogTasks
            .Include(x => x.Assignees)
            .Include(x => x.TimeEntries)
            .Where(x => !x.IsCompleted && !x.IsArchived)
            .ToListAsync(ct);

        return entities
            .Select(x => x.ToDomain())
            .ToList();
    }

    public async Task<IReadOnlyCollection<BacklogTask>> GetByTeamAsync(Guid teamId, CancellationToken ct = default)
    {
        var entities = await db.BacklogTasks
            .Include(x => x.Assignees)
            .Include(x => x.TimeEntries)
            .Where(x => x.TeamId == teamId && !x.IsArchived)
            .ToListAsync(ct);

        return entities
            .Select(x => x.ToDomain())
            .ToList();
    }

    public async Task<IReadOnlyCollection<BacklogTask>> GetActiveByTeamAsync(Guid teamId, CancellationToken ct = default)
    {
        var entities = await db.BacklogTasks
            .Include(x => x.Assignees)
            .Include(x => x.TimeEntries)
            .Where(x => x.TeamId == teamId && !x.IsCompleted && !x.IsArchived)
            .ToListAsync(ct);

        return entities
            .Select(x => x.ToDomain())
            .ToList();
    }

    public async Task<IReadOnlyCollection<BacklogTask>> GetOverdueUnnotifiedAsync(DateTime utcNow, CancellationToken ct = default)
    {
        var entities = await db.BacklogTasks
            .Include(x => x.Assignees)
            .Include(x => x.TimeEntries)
            .Where(x =>
                !x.IsCompleted &&
                !x.IsArchived &&
                x.DueDate.HasValue &&
                x.DueDate.Value < utcNow &&
                x.OverdueNotifiedAt == null)
            .ToListAsync(ct);

        return entities
            .Select(x => x.ToDomain())
            .ToList();
    }

    public async Task<BacklogTask?> UpdateAsync(BacklogTask task, CancellationToken ct = default)
    {
        var entity = await db.BacklogTasks
            .Include(x => x.Assignees)
            .Include(x => x.TimeEntries)
            .FirstOrDefaultAsync(x => x.Id == task.Id.Id, ct);

        if (entity is null)
        {
            return null;
        }

        MapToExistingEntity(task, entity);

        await db.SaveChangesAsync(ct);

        return entity.ToDomain();
    }

    public async Task<Domain.Entities.TimeEntry> AddTimeEntryAsync(Domain.Entities.TimeEntry entry, CancellationToken ct = default)
    {
        var entity = entry.ToEntity();
        await db.TimeEntries.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task<bool> DeleteTimeEntryAsync(Guid entryId, Guid backlogTaskId, CancellationToken ct = default)
    {
        var entity = await db.TimeEntries
            .FirstOrDefaultAsync(x => x.Id == entryId && x.BacklogTaskId == backlogTaskId, ct);

        if (entity is null)
            return false;

        db.TimeEntries.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static void MapToExistingEntity(BacklogTask source, BacklogTaskEntity target)
    {
        target.Title = source.Title;
        target.Priority = source.Priority;
        target.Description = source.Description;
        target.TeamId = source.TeamId.Id;
        target.InWeek = source.InWeek;
        target.IsCompleted = source.IsCompleted;
        target.IsArchived = source.IsArchived;
        target.DueDate = source.DueDate;
        target.EstimateMinutes = source.EstimateMinutes;
        target.ProgressPct = source.ProgressPct;
        target.OverdueNotifiedAt = source.OverdueNotifiedAt;

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