using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface IAccountRepository
{
    Task<List<Account>> GetByTeamAsync(Guid teamId, CancellationToken ct = default);
    Task<Account?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Account> CreateAsync(Account account, CancellationToken ct = default);
    Task<Account> UpdateAsync(Account account, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
