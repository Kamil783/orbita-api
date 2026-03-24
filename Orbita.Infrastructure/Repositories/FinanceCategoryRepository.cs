using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class FinanceCategoryRepository(OrbitaDbContext db) : IFinanceCategoryRepository
{
    public async Task<List<FinanceCategory>> GetByUserAsync(Guid userId, CancellationToken ct = default)
    {
        var entities = await db.FinanceCategories
            .Where(x => x.CreatorId == userId)
            .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<FinanceCategory?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.FinanceCategories
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return entity?.ToDomain();
    }

    public async Task<FinanceCategory> CreateAsync(FinanceCategory category, CancellationToken ct = default)
    {
        var entity = category.ToEntity();
        await db.FinanceCategories.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }
}
