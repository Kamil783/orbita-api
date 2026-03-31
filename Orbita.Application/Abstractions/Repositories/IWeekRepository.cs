using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IWeekRepository
{
    Task<Week?> GetCurrentAsync(Guid teamId, CancellationToken ct = default);
    Task<List<Week>> GetArchivedAsync(Guid teamId, CancellationToken ct = default);
    Task<Week> CreateAsync(Week week, CancellationToken ct = default);
    Task<Week?> UpdateAsync(Week week, CancellationToken ct = default);
    Task<List<Week>> GetByBacklogTaskAsync(Guid backlogTaskId, CancellationToken ct = default);
}
