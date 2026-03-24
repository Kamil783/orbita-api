using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class FinanceCategory
{
    public FinanceCategoryId Id { get; private set; }
    public UserId CreatorId { get; private set; }
    public string Name { get; private set; }
    public string Icon { get; private set; }
    public string Bg { get; private set; }
    public string Color { get; private set; }
    public long? WeeklyLimit { get; private set; }
    public long? MonthlyLimit { get; private set; }

    private FinanceCategory() { }

    public static FinanceCategory Create(
        UserId creatorId,
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
            Name = name,
            Icon = icon,
            Bg = bg,
            Color = color,
            WeeklyLimit = weeklyLimit,
            MonthlyLimit = monthlyLimit
        };
    }
}
