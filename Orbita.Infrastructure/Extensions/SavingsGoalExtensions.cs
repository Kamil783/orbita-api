using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class SavingsGoalExtensions
{
    public static SavingsGoalEntity ToEntity(this SavingsGoal goal)
    {
        return new SavingsGoalEntity
        {
            Id = goal.Id.Id,
            CreatorId = goal.CreatorId.Id,
            TeamId = goal.TeamId.Id,
            Name = goal.Name,
            Target = goal.Target,
            Current = goal.Current
        };
    }

    public static SavingsGoal ToDomain(this SavingsGoalEntity entity)
    {
        return SavingsGoal.Restore(
            id: new SavingsGoalId(entity.Id),
            creatorId: new UserId(entity.CreatorId),
            teamId: new TeamId(entity.TeamId),
            name: entity.Name,
            target: entity.Target,
            current: entity.Current
        );
    }
}
