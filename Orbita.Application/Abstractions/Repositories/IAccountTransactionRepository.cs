using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IAccountTransactionRepository
{
    Task<List<AccountTransaction>> GetByAccountAsync(Guid accountId, CancellationToken ct = default);
    Task<List<AccountTransaction>> GetByTeamAsync(Guid teamId, CancellationToken ct = default);
    Task<AccountTransaction?> GetAsync(Guid id, CancellationToken ct = default);
    Task<AccountTransaction> CreateAsync(AccountTransaction transaction, CancellationToken ct = default);
    Task<AccountTransaction> UpdateAsync(AccountTransaction transaction, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
