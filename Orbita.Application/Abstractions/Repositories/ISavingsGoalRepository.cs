using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface ISavingsGoalRepository
{
    Task<List<SavingsGoal>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<SavingsGoal> CreateAsync(SavingsGoal goal, CancellationToken ct = default);
}
