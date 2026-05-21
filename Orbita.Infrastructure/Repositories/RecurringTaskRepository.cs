using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class RecurringTaskRepository(OrbitaDbContext db) : IRecurringTaskRepository
{
    public async Task<List<RecurringTask>> GetByTeamAsync(Guid teamId, CancellationToken ct = default)
    {
        var entities = await db.RecurringTasks
            .Where(x => x.TeamId == teamId)
            .OrderBy(x => x.DayOfMonth)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<RecurringTask?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.RecurringTasks.FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity?.ToDomain();
    }

    public async Task<RecurringTask> CreateAsync(RecurringTask task, CancellationToken ct = default)
    {
        var entity = task.ToEntity();
        await db.RecurringTasks.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task<RecurringTask> UpdateAsync(RecurringTask task, CancellationToken ct = default)
    {
        var entity = await db.RecurringTasks.FirstOrDefaultAsync(x => x.Id == task.Id.Id, ct);
        if (entity is null)
            throw new InvalidOperationException("Recurring task not found.");

        entity.Title = task.Title;
        entity.Description = task.Description;
        entity.DayOfMonth = task.DayOfMonth;
        entity.IsCompleted = task.IsCompleted;
        entity.LastResetAt = task.LastResetAt;
        entity.LastOverdueNotifiedAt = task.LastOverdueNotifiedAt;
        entity.UpdatedAt = task.UpdatedAt;

        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.RecurringTasks.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            db.RecurringTasks.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task<int> ResetCompletedForCycleAsync(DateTime cycleStart, DateTime utcNow, CancellationToken ct = default)
    {
        return await db.RecurringTasks
            .Where(x => x.IsCompleted &&
                        (x.LastResetAt == null || x.LastResetAt < cycleStart))
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.IsCompleted, false)
                .SetProperty(x => x.LastResetAt, utcNow)
                .SetProperty(x => x.LastOverdueNotifiedAt, (DateTime?)null)
                .SetProperty(x => x.UpdatedAt, utcNow),
                ct);
    }

    public async Task<List<RecurringTask>> GetOverdueNotNotifiedTodayAsync(DateTime utcNow, CancellationToken ct = default)
    {
        var startOfToday = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);

        // SQL-фильтр: только активные кандидаты (не выполнено + ещё не уведомляли сегодня).
        // Точную проверку «overdue с учётом клампа DayOfMonth до days-in-month» дополним в C#.
        var candidates = await db.RecurringTasks
            .Where(x => !x.IsCompleted &&
                        (x.LastOverdueNotifiedAt == null || x.LastOverdueNotifiedAt < startOfToday))
            .ToListAsync(ct);

        return candidates
            .Select(x => x.ToDomain())
            .Where(x => x.IsOverdueOn(utcNow))
            .ToList();
    }
}
