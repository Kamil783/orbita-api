using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class AccountRepository(OrbitaDbContext db) : IAccountRepository
{
    public async Task<List<Account>> GetByTeamAsync(Guid teamId, CancellationToken ct = default)
    {
        var entities = await db.Accounts
            .Where(x => x.TeamId == teamId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<Account?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Accounts.FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity?.ToDomain();
    }

    public async Task<Account> CreateAsync(Account account, CancellationToken ct = default)
    {
        var entity = account.ToEntity();
        await db.Accounts.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task<Account> UpdateAsync(Account account, CancellationToken ct = default)
    {
        var entity = await db.Accounts.FirstOrDefaultAsync(x => x.Id == account.Id.Id, ct);
        if (entity is null)
            throw new InvalidOperationException("Account not found.");

        entity.Name = account.Name;
        entity.CurrencyCode = account.CurrencyCode;
        entity.Balance = account.Balance;
        entity.UpdatedAt = account.UpdatedAt;

        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Accounts.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            db.Accounts.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}
