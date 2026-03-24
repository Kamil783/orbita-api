using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class SpendingLimitRepository(OrbitaDbContext db) : ISpendingLimitRepository
{
    public async Task<SpendingLimit?> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var entity = await db.SpendingLimits
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

        return entity?.ToDomain();
    }

    public async Task<SpendingLimit> CreateAsync(SpendingLimit limit, CancellationToken ct = default)
    {
        var entity = limit.ToEntity();
        await db.SpendingLimits.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task<SpendingLimit> UpdateAsync(SpendingLimit limit, CancellationToken ct = default)
    {
        var entity = await db.SpendingLimits
            .FirstOrDefaultAsync(x => x.UserId == limit.UserId.Id, ct);

        if (entity is null)
        {
            entity = limit.ToEntity();
            await db.SpendingLimits.AddAsync(entity, ct);
        }
        else
        {
            entity.MonthlyLimit = limit.MonthlyLimit;
            entity.WeeklyLimit = limit.WeeklyLimit;
        }

        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }
}
