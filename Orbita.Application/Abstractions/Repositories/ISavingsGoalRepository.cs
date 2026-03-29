using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Repositories;

public interface ISavingsGoalRepository
{
    Task<SavingsGoal?> GetAsync(Guid id, CancellationToken ct = default);
    Task<List<SavingsGoal>> GetByTeamAsync(Guid teamId, CancellationToken ct = default);
    Task<SavingsGoal> CreateAsync(SavingsGoal goal, CancellationToken ct = default);
    Task<SavingsGoal> UpdateAsync(SavingsGoal goal, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
