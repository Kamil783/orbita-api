using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class FinanceBalance
{
    public UserId UserId { get; private set; }
    public long Balance { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private FinanceBalance() { }

    public static FinanceBalance Create(UserId userId)
    {
        return new FinanceBalance
        {
            UserId = userId,
            Balance = 0,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static FinanceBalance Restore(UserId userId, long balance, DateTime updatedAt)
    {
        return new FinanceBalance
        {
            UserId = userId,
            Balance = balance,
            UpdatedAt = updatedAt
        };
    }

    public void Adjust(long delta)
    {
        Balance += delta;
        UpdatedAt = DateTime.UtcNow;
    }
}
