using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class RecurringPaymentRepository(OrbitaDbContext db) : IRecurringPaymentRepository
{
    public async Task<List<RecurringPayment>> GetByTeamAsync(Guid teamId, CancellationToken ct = default)
    {
        var entities = await db.RecurringPayments
            .Where(x => x.TeamId == teamId)
            .OrderBy(x => x.DayOfMonth)
            .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<RecurringPayment?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.RecurringPayments.FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity?.ToDomain();
    }

    public async Task<RecurringPayment> CreateAsync(RecurringPayment payment, CancellationToken ct = default)
    {
        var entity = payment.ToEntity();
        await db.RecurringPayments.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task<RecurringPayment> UpdateAsync(RecurringPayment payment, CancellationToken ct = default)
    {
        var entity = await db.RecurringPayments.FirstOrDefaultAsync(x => x.Id == payment.Id.Id, ct);
        if (entity is null)
            throw new InvalidOperationException("Recurring payment not found.");

        entity.Title = payment.Title;
        entity.Amount = payment.Amount;
        entity.DayOfMonth = payment.DayOfMonth;
        entity.CategoryId = payment.CategoryId?.Id;
        entity.UpdatedAt = payment.UpdatedAt;

        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.RecurringPayments.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            db.RecurringPayments.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}
