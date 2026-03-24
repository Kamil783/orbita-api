using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IFinanceTransactionRepository
{
    Task<List<FinanceTransaction>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<FinanceTransaction?> GetAsync(Guid id, CancellationToken ct = default);
    Task<FinanceTransaction> CreateAsync(FinanceTransaction transaction, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<List<FinanceTransaction>> GetByUserInPeriodAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct = default);
}
