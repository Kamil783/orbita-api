using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class FinanceCategory
{
    public FinanceCategoryId Id { get; private set; }
    public UserId CreatorId { get; private set; }
    public TeamId TeamId { get; private set; }
    public string Name { get; private set; }
    public string Icon { get; private set; }
    public string Bg { get; private set; }
    public string Color { get; private set; }
    public long? WeeklyLimit { get; private set; }
    public long? MonthlyLimit { get; private set; }

    private FinanceCategory() { }

    public static FinanceCategory Create(
        UserId creatorId,
        TeamId teamId,
        string name,
        string icon,
        string bg,
        string color,
        long? weeklyLimit = null,
        long? monthlyLimit = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        return new FinanceCategory
        {
            Id = new FinanceCategoryId(Guid.NewGuid()),
            CreatorId = creatorId,
            TeamId = teamId,
            Name = name,
            Icon = icon,
            Bg = bg,
            Color = color,
            WeeklyLimit = weeklyLimit,
            MonthlyLimit = monthlyLimit
        };
    }

    public static FinanceCategory Restore(
        FinanceCategoryId id,
        UserId creatorId,
        TeamId teamId,
        string name,
        string icon,
        string bg,
        string color,
        long? weeklyLimit,
        long? monthlyLimit)
    {
        return new FinanceCategory
        {
            Id = id,
            CreatorId = creatorId,
            TeamId = teamId,
            Name = name,
            Icon = icon,
            Bg = bg,
            Color = color,
            WeeklyLimit = weeklyLimit,
            MonthlyLimit = monthlyLimit
        };
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        Name = name;
    }

    public void SetIcon(string icon)
    {
        Icon = icon ?? throw new ArgumentNullException(nameof(icon));
    }

    public void SetBg(string bg)
    {
        Bg = bg ?? throw new ArgumentNullException(nameof(bg));
    }

    public void SetColor(string color)
    {
        Color = color ?? throw new ArgumentNullException(nameof(color));
    }

    public void SetWeeklyLimit(long? weeklyLimit)
    {
        if (weeklyLimit < 0)
            throw new ArgumentException("Weekly limit cannot be negative.", nameof(weeklyLimit));

        WeeklyLimit = weeklyLimit;
    }

    public void SetMonthlyLimit(long? monthlyLimit)
    {
        if (monthlyLimit < 0)
            throw new ArgumentException("Monthly limit cannot be negative.", nameof(monthlyLimit));

        MonthlyLimit = monthlyLimit;
    }

    public void SetLimits(long? weeklyLimit, long? monthlyLimit)
    {
        if (weeklyLimit < 0)
            throw new ArgumentException("Weekly limit cannot be negative.", nameof(weeklyLimit));

        if (monthlyLimit < 0)
            throw new ArgumentException("Monthly limit cannot be negative.", nameof(monthlyLimit));

        WeeklyLimit = weeklyLimit;
        MonthlyLimit = monthlyLimit;
    }
}
