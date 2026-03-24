using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class SavingsGoalRepository(OrbitaDbContext db) : ISavingsGoalRepository
{
    public async Task<List<SavingsGoal>> GetByUserAsync(Guid userId, CancellationToken ct = default)
    {
        var entities = await db.SavingsGoals
            .Where(x => x.CreatorId == userId)
            .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<SavingsGoal?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.SavingsGoals.FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity?.ToDomain();
    }

    public async Task<SavingsGoal> CreateAsync(SavingsGoal goal, CancellationToken ct = default)
    {
        var entity = goal.ToEntity();
        await db.SavingsGoals.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task<SavingsGoal> UpdateAsync(SavingsGoal goal, CancellationToken ct = default)
    {
        var entity = await db.SavingsGoals.FirstOrDefaultAsync(x => x.Id == goal.Id.Id, ct);
        if (entity is null) throw new InvalidOperationException("Savings goal not found.");

        entity.Name = goal.Name;
        entity.Target = goal.Target;
        entity.Current = goal.Current;
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.SavingsGoals.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is not null)
        {
            db.SavingsGoals.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}
