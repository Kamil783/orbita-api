using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IBacklogTaskRepository
{
    Task<BacklogTask> Get(Guid id);
    Task<IReadOnlyCollection<BacklogTask>> GetAll();
    Task<BacklogTask> Create(BacklogTask task);
    Task<BacklogTask> Update(BacklogTask task);
    Task<BacklogTask> Delete(Guid id);
}
