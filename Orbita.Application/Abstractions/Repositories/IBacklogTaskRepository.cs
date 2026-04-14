using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IBacklogTaskRepository
{
    Task<BacklogTask?> GetAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<BacklogTask>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyCollection<BacklogTask>> GetAllActiveAsync(CancellationToken ct);
    Task<IReadOnlyCollection<BacklogTask>> GetByTeamAsync(Guid teamId, CancellationToken ct);
    Task<BacklogTask> CreateAsync(BacklogTask task, CancellationToken ct);
    Task<BacklogTask?> UpdateAsync(BacklogTask task, CancellationToken ct);
    Task<BacklogTask?> DeleteAsync(Guid id, CancellationToken ct);
    Task<TimeEntry> AddTimeEntryAsync(TimeEntry entry, CancellationToken ct);
    Task<bool> DeleteTimeEntryAsync(Guid entryId, Guid backlogTaskId, CancellationToken ct);
}
