using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IRecurringTaskRepository
{
    Task<List<RecurringTask>> GetByTeamAsync(Guid teamId, CancellationToken ct = default);
    Task<RecurringTask?> GetAsync(Guid id, CancellationToken ct = default);
    Task<RecurringTask> CreateAsync(RecurringTask task, CancellationToken ct = default);
    Task<RecurringTask> UpdateAsync(RecurringTask task, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    /// <summary>
    /// Сбрасывает IsCompleted в false для всех записей, у которых LastResetAt &lt; <paramref name="cycleStart"/>
    /// (или null). Возвращает количество затронутых записей.
    /// </summary>
    Task<int> ResetCompletedForCycleAsync(DateTime cycleStart, DateTime utcNow, CancellationToken ct = default);

    /// <summary>
    /// Возвращает просроченные (с учётом клампа DayOfMonth до days-in-month и !IsCompleted)
    /// задачи, по которым сегодня ещё не отправляли уведомление.
    /// </summary>
    Task<List<RecurringTask>> GetOverdueNotNotifiedTodayAsync(DateTime utcNow, CancellationToken ct = default);
}
