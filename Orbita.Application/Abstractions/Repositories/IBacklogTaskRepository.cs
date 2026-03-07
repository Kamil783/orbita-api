using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IBacklogTaskRepository
{
    Task<BacklogTask?> Get(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<BacklogTask>> GetAll(CancellationToken ct);
    Task<BacklogTask> Create(BacklogTask task, CancellationToken ct);
    Task<BacklogTask?> Update(BacklogTask task, CancellationToken ct);
    Task<BacklogTask?> Delete(Guid id, CancellationToken ct);
}
