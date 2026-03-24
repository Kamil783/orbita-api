using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class FinanceTransaction
{
    public FinanceTransactionId Id { get; private set; }
    public UserId CreatorId { get; private set; }
    public FinanceCategoryId CategoryId { get; private set; }
    public string Title { get; private set; }
    public long Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private FinanceTransaction() { }

    public static FinanceTransaction Create(
        UserId creatorId,
        FinanceCategoryId categoryId,
        string title,
        long amount)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        return new FinanceTransaction
        {
            Id = new FinanceTransactionId(Guid.NewGuid()),
            CreatorId = creatorId,
            CategoryId = categoryId,
            Title = title,
            Amount = amount,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static FinanceTransaction Restore(
        FinanceTransactionId id,
        UserId creatorId,
        FinanceCategoryId categoryId,
        string title,
        long amount,
        DateTime createdAt)
    {
        return new FinanceTransaction
        {
            Id = id,
            CreatorId = creatorId,
            CategoryId = categoryId,
            Title = title,
            Amount = amount,
            CreatedAt = createdAt
        };
    }
}
