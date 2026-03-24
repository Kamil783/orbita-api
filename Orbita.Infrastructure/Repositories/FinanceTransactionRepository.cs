using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class FinanceTransactionRepository(OrbitaDbContext db) : IFinanceTransactionRepository
{
    public async Task<List<FinanceTransaction>> GetByUserAsync(Guid userId, CancellationToken ct = default)
    {
        var entities = await db.FinanceTransactions
            .Where(x => x.CreatorId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<FinanceTransaction?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.FinanceTransactions
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return entity?.ToDomain();
    }

    public async Task<FinanceTransaction> CreateAsync(FinanceTransaction transaction, CancellationToken ct = default)
    {
        var entity = transaction.ToEntity();
        await db.FinanceTransactions.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.FinanceTransactions
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity is not null)
        {
            db.FinanceTransactions.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task<List<FinanceTransaction>> GetByUserInPeriodAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        var entities = await db.FinanceTransactions
            .Where(x => x.CreatorId == userId && x.CreatedAt >= from && x.CreatedAt < to)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }
}
