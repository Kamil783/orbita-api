using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IFinanceCategoryRepository
{
    Task<List<FinanceCategory>> GetByTeamAsync(Guid teamId, CancellationToken ct = default);
    Task<FinanceCategory?> GetAsync(Guid id, CancellationToken ct = default);
    Task<FinanceCategory> CreateAsync(FinanceCategory category, CancellationToken ct = default);
    Task<FinanceCategory?> UpdateAsync(FinanceCategory category, CancellationToken ct = default);
}
