using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IBacklogTaskRepository
{
    Task<BacklogTask?> GetAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<BacklogTask>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyCollection<BacklogTask>> GetAllActiveAsync(CancellationToken ct);
    Task<IReadOnlyCollection<BacklogTask>> GetByTeamAsync(Guid teamId, CancellationToken ct);
    Task<IReadOnlyCollection<BacklogTask>> GetActiveByTeamAsync(Guid teamId, CancellationToken ct);
    /// <summary>Активные (не завершённые, не архивированные) задачи с DueDate &lt; <paramref name="utcNow"/> и ещё не уведомлённые о просрочке.</summary>
    Task<IReadOnlyCollection<BacklogTask>> GetOverdueUnnotifiedAsync(DateTime utcNow, CancellationToken ct);
    Task<BacklogTask> CreateAsync(BacklogTask task, CancellationToken ct);
    Task<BacklogTask?> UpdateAsync(BacklogTask task, CancellationToken ct);
    Task<BacklogTask?> DeleteAsync(Guid id, CancellationToken ct);
    Task DeleteBatchAsync(IEnumerable<Guid> id, CancellationToken ct);
    Task ArchiveBatchAsync(IEnumerable<Guid> ids, CancellationToken ct);
    Task<TimeEntry> AddTimeEntryAsync(TimeEntry entry, CancellationToken ct);
    Task<bool> DeleteTimeEntryAsync(Guid entryId, Guid backlogTaskId, CancellationToken ct);
}
