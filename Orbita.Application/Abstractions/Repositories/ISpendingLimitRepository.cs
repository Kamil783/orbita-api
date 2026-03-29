using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface ISpendingLimitRepository
{
    Task<SpendingLimit?> GetAsync(Guid teamId, CancellationToken ct = default);
    Task<SpendingLimit> CreateAsync(SpendingLimit limit, CancellationToken ct = default);
    Task<SpendingLimit> UpdateAsync(SpendingLimit limit, CancellationToken ct = default);
}
