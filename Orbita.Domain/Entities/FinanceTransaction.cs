using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class FinanceTransaction
{
    public FinanceTransactionId Id { get; private set; }
    public UserId CreatorId { get; private set; }
    public TeamId TeamId { get; private set; }
    public FinanceCategoryId? CategoryId { get; private set; }
    public string Title { get; private set; }
    public long Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsFromBalance { get; private set; }

    private FinanceTransaction() { }

    public static FinanceTransaction Create(
        UserId creatorId,
        TeamId teamId,
        FinanceCategoryId? categoryId,
        string title,
        long amount,
        bool isFromBalance,
        DateTime? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        return new FinanceTransaction
        {
            Id = new FinanceTransactionId(Guid.NewGuid()),
            CreatorId = creatorId,
            TeamId = teamId,
            CategoryId = categoryId,
            Title = title,
            Amount = amount,
            CreatedAt = createdAt ?? DateTime.UtcNow,
            IsFromBalance = isFromBalance
        };
    }

    public void SetCategoryId(FinanceCategoryId? categoryId)
    {
        CategoryId = categoryId;
    }

    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        Title = title;
    }

    public void SetAmount(long amount)
    {
        Amount = amount;
    }

    public void SetIsFromBalance(bool isFromBalance)
    {
        IsFromBalance = isFromBalance;
    }
    public void SetCreatedAt(DateTime createdAt)
    {
        CreatedAt = createdAt;
    }

    public static FinanceTransaction Restore(
        FinanceTransactionId id,
        UserId creatorId,
        TeamId teamId,
        FinanceCategoryId? categoryId,
        string title,
        long amount,
        DateTime createdAt,
        bool isFromBalance)
    {
        return new FinanceTransaction
        {
            Id = id,
            CreatorId = creatorId,
            TeamId = teamId,
            CategoryId = categoryId,
            Title = title,
            Amount = amount,
            CreatedAt = createdAt,
            IsFromBalance = isFromBalance
        };
    }
}
