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
            UserId = limit.UserId.Id,
            MonthlyLimit = limit.MonthlyLimit,
            WeeklyLimit = limit.WeeklyLimit
        };
    }

    public static SpendingLimit ToDomain(this SpendingLimitEntity entity)
    {
        return SpendingLimit.Restore(
            userId: new UserId(entity.UserId),
            monthlyLimit: entity.MonthlyLimit,
            weeklyLimit: entity.WeeklyLimit
        );
    }
}
