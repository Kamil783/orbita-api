using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class SpendingLimitExtensions
{
    public static SpendingLimitEntity ToEntity(this SpendingLimit limit)
    {
        return new SpendingLimitEntity
        {
            TeamId = limit.TeamId.Id,
            MonthlyLimit = limit.MonthlyLimit,
            WeeklyLimit = limit.WeeklyLimit
        };
    }

    public static SpendingLimit ToDomain(this SpendingLimitEntity entity)
    {
        return SpendingLimit.Restore(
            teamId: new TeamId(entity.TeamId),
            monthlyLimit: entity.MonthlyLimit,
            weeklyLimit: entity.WeeklyLimit
        );
    }
}
