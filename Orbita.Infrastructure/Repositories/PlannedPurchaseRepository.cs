using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class PlannedPurchaseRepository(OrbitaDbContext db) : IPlannedPurchaseRepository
{
    public async Task<List<PlannedPurchase>> GetByTeamAsync(
        Guid teamId,
        DateOnly? from,
        DateOnly? to,
        PlannedPurchaseStatus? status,
        Guid? assigneeId,
        Guid? categoryId,
        CancellationToken ct = default)
    {
        var query = db.PlannedPurchases.AsQueryable()
            .Where(x => x.TeamId == teamId);

        if (from.HasValue)
            query = query.Where(x => x.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.Date <= to.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (assigneeId.HasValue)
            query = query.Where(x => x.AssigneeId == assigneeId.Value);

        if (categoryId.HasValue)
            query = query.Where(x => x.CategoryId == categoryId.Value);

        var entities = await query
            .OrderBy(x => x.Date)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<PlannedPurchase?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.PlannedPurchases.FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity?.ToDomain();
    }

    public async Task<PlannedPurchase> CreateAsync(PlannedPurchase purchase, CancellationToken ct = default)
    {
        var entity = purchase.ToEntity();
        await db.PlannedPurchases.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task<PlannedPurchase> UpdateAsync(PlannedPurchase purchase, CancellationToken ct = default)
    {
        var entity = await db.PlannedPurchases.FirstOrDefaultAsync(x => x.Id == purchase.Id.Id, ct);
        if (entity is null)
            throw new InvalidOperationException("Planned purchase not found.");

        entity.Title = purchase.Title;
        entity.Date = purchase.Date;
        entity.Amount = purchase.Amount;
        entity.AssigneeId = purchase.AssigneeId?.Id;
        entity.CategoryId = purchase.CategoryId?.Id;
        entity.Note = purchase.Note;
        entity.Status = purchase.Status;
        entity.UpdatedAt = purchase.UpdatedAt;

        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.PlannedPurchases.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            db.PlannedPurchases.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}
