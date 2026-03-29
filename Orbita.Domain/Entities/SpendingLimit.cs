using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class SpendingLimit
{
    public UserId UserId { get; private set; }
    public TeamId TeamId { get; private set; }
    public long MonthlyLimit { get; private set; }
    public long WeeklyLimit { get; private set; }

    private SpendingLimit() { }

    public static SpendingLimit Create(UserId userId)
    {
        return new SpendingLimit
        {
            UserId = userId,
            MonthlyLimit = 0,
            WeeklyLimit = 0
        };
    }

    public static SpendingLimit Restore(UserId userId, long monthlyLimit, long weeklyLimit)
    {
        return new SpendingLimit
        {
            UserId = userId,
            MonthlyLimit = monthlyLimit,
            WeeklyLimit = weeklyLimit
        };
    }

    public void Update(long monthlyLimit, long weeklyLimit)
    {
        MonthlyLimit = monthlyLimit;
        WeeklyLimit = weeklyLimit;
    }
}
