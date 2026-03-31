using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class FinanceBalance
{
    public TeamId TeamId { get; private set; }
    public long Balance { get; private set; }
    public long PreviousMonthBalance { get; private set; }
    public DateTime? LastMonthClosedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private FinanceBalance() { }

    public static FinanceBalance Create(TeamId teamId)
    {
        return new FinanceBalance
        {
            TeamId = teamId,
            Balance = 0,
            PreviousMonthBalance = 0,
            LastMonthClosedAt = null,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static FinanceBalance Restore(
        TeamId teamId,
        long balance,
        long previousMonthBalance,
        DateTime? lastMonthClosedAt,
        DateTime updatedAt)
    {
        return new FinanceBalance
        {
            TeamId = teamId,
            Balance = balance,
            PreviousMonthBalance = previousMonthBalance,
            LastMonthClosedAt = lastMonthClosedAt,
            UpdatedAt = updatedAt
        };
    }

    public void Adjust(long delta)
    {
        Balance += delta;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CloseMonth()
    {
        PreviousMonthBalance = Balance;
        LastMonthClosedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
