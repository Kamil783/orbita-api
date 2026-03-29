using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IFinanceTransactionRepository
{
    Task<List<FinanceTransaction>> GetByTeamAsync(Guid teamId, CancellationToken ct = default);
    Task<FinanceTransaction?> GetAsync(Guid id, CancellationToken ct = default);
    Task<FinanceTransaction> CreateAsync(FinanceTransaction transaction, CancellationToken ct = default);
    Task<FinanceTransaction?> UpdateAsync(FinanceTransaction transaction, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<List<FinanceTransaction>> GetByTeamInPeriodAsync(Guid teamId, DateTime from, DateTime to, CancellationToken ct = default);
}
