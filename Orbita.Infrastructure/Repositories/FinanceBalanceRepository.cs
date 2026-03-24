using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class FinanceBalanceRepository(OrbitaDbContext db) : IFinanceBalanceRepository
{
    public async Task<FinanceBalance?> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var entity = await db.FinanceBalances
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

        return entity?.ToDomain();
    }

    public async Task<List<FinanceBalance>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await db.FinanceBalances.ToListAsync(ct);
        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<FinanceBalance> CreateAsync(FinanceBalance balance, CancellationToken ct = default)
    {
        var entity = balance.ToEntity();
        await db.FinanceBalances.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task<FinanceBalance> UpdateAsync(FinanceBalance balance, CancellationToken ct = default)
    {
        var entity = await db.FinanceBalances
            .FirstOrDefaultAsync(x => x.UserId == balance.UserId.Id, ct);

        if (entity is null)
        {
            entity = balance.ToEntity();
            await db.FinanceBalances.AddAsync(entity, ct);
        }
        else
        {
            entity.Balance = balance.Balance;
            entity.PreviousMonthBalance = balance.PreviousMonthBalance;
            entity.LastMonthClosedAt = balance.LastMonthClosedAt;
            entity.UpdatedAt = balance.UpdatedAt;
        }

        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }
}
