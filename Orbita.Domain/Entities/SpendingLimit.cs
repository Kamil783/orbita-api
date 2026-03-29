using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class SpendingLimit
{
    public TeamId TeamId { get; private set; }
    public long MonthlyLimit { get; private set; }
    public long WeeklyLimit { get; private set; }

    private SpendingLimit() { }

    public static SpendingLimit Create(TeamId teamId)
    {
        return new SpendingLimit
        {
            TeamId = teamId,
            MonthlyLimit = 0,
            WeeklyLimit = 0
        };
    }

    public static SpendingLimit Restore(TeamId teamId, long monthlyLimit, long weeklyLimit)
    {
        return new SpendingLimit
        {
            TeamId = teamId,
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
