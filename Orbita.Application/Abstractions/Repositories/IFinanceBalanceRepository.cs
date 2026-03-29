using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IFinanceBalanceRepository
{
    Task<FinanceBalance?> GetAsync(Guid teamId, CancellationToken ct = default);
    Task<List<FinanceBalance>> GetAllAsync(CancellationToken ct = default);
    Task<FinanceBalance> CreateAsync(FinanceBalance balance, CancellationToken ct = default);
    Task<FinanceBalance> UpdateAsync(FinanceBalance balance, CancellationToken ct = default);
}
