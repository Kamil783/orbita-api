using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class AccountTransactionRepository(OrbitaDbContext db) : IAccountTransactionRepository
{
    public async Task<List<AccountTransaction>> GetByAccountAsync(Guid accountId, CancellationToken ct = default)
    {
        var entities = await db.AccountTransactions
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<List<AccountTransaction>> GetByTeamAsync(Guid teamId, CancellationToken ct = default)
    {
        var entities = await db.AccountTransactions
            .Where(x => x.TeamId == teamId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<AccountTransaction?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.AccountTransactions.FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity?.ToDomain();
    }

    public async Task<AccountTransaction> CreateAsync(AccountTransaction transaction, CancellationToken ct = default)
    {
        var entity = transaction.ToEntity();
        await db.AccountTransactions.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task<AccountTransaction> UpdateAsync(AccountTransaction transaction, CancellationToken ct = default)
    {
        var entity = await db.AccountTransactions.FirstOrDefaultAsync(x => x.Id == transaction.Id.Id, ct);
        if (entity is null)
            throw new InvalidOperationException("Account transaction not found.");

        entity.CategoryId = transaction.CategoryId?.Id;
        entity.Title = transaction.Title;
        entity.Amount = transaction.Amount;
        entity.CreatedAt = transaction.CreatedAt;

        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.AccountTransactions.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            db.AccountTransactions.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}
