using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IColumnRepository
{
    Task<Column?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyCollection<Column>> GetAllWithItemsAsync(Guid teamId, CancellationToken ct = default);
    Task<Column> CreateAsync(Column column, CancellationToken ct = default);
}
