using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class RecurringPayment
{
    public RecurringPaymentId Id { get; private set; }
    public UserId CreatorId { get; private set; }
    public TeamId TeamId { get; private set; }
    public string Title { get; private set; }
    public long Amount { get; private set; }
    public int DayOfMonth { get; private set; }
    public FinanceCategoryId? CategoryId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private RecurringPayment() { }

    public static RecurringPayment Create(
        UserId creatorId,
        TeamId teamId,
        string title,
        long amount,
        int dayOfMonth,
        FinanceCategoryId? categoryId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");

        if (dayOfMonth < 1 || dayOfMonth > 31)
            throw new ArgumentOutOfRangeException(nameof(dayOfMonth), "DayOfMonth must be between 1 and 31.");

        var now = DateTime.UtcNow;
        return new RecurringPayment
        {
            Id = new RecurringPaymentId(Guid.NewGuid()),
            CreatorId = creatorId,
            TeamId = teamId,
            Title = title,
            Amount = amount,
            DayOfMonth = dayOfMonth,
            CategoryId = categoryId,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public static RecurringPayment Restore(
        RecurringPaymentId id,
        UserId creatorId,
        TeamId teamId,
        string title,
        long amount,
        int dayOfMonth,
        FinanceCategoryId? categoryId,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new RecurringPayment
        {
            Id = id,
            CreatorId = creatorId,
            TeamId = teamId,
            Title = title,
            Amount = amount,
            DayOfMonth = dayOfMonth,
            CategoryId = categoryId,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public void Update(string? title, long? amount, int? dayOfMonth, FinanceCategoryId? categoryId, bool clearCategory)
    {
        if (title is not null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.", nameof(title));
            Title = title;
        }

        if (amount.HasValue)
        {
            if (amount.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");
            Amount = amount.Value;
        }

        if (dayOfMonth.HasValue)
        {
            if (dayOfMonth.Value < 1 || dayOfMonth.Value > 31)
                throw new ArgumentOutOfRangeException(nameof(dayOfMonth), "DayOfMonth must be between 1 and 31.");
            DayOfMonth = dayOfMonth.Value;
        }

        if (clearCategory)
            CategoryId = null;
        else if (categoryId is not null)
            CategoryId = categoryId;

        UpdatedAt = DateTime.UtcNow;
    }
}
