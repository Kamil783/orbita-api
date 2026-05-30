using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

/// <summary>
/// Транзакция по конкретному <see cref="Account"/>. В отличие от <see cref="FinanceTransaction"/>,
/// никогда не списывает с общего баланса команды (IsFromBalance отсутствует) — всегда меняет
/// баланс именно привязанного счёта в его валюте.
/// </summary>
public class AccountTransaction
{
    public AccountTransactionId Id { get; private set; } = default!;
    public AccountId AccountId { get; private set; } = default!;
    public UserId CreatorId { get; private set; } = default!;
    public TeamId TeamId { get; private set; } = default!;
    public FinanceCategoryId? CategoryId { get; private set; }
    public string Title { get; private set; } = default!;
    /// <summary>Знаковая сумма в валюте счёта. Отрицательная — расход, положительная — поступление.</summary>
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AccountTransaction() { }

    public static AccountTransaction Create(
        AccountId accountId,
        UserId creatorId,
        TeamId teamId,
        FinanceCategoryId? categoryId,
        string title,
        decimal amount,
        DateTime createdAt)
    {
        ValidateTitle(title);
        ValidateAmount(amount);

        return new AccountTransaction
        {
            Id = new AccountTransactionId(Guid.NewGuid()),
            AccountId = accountId,
            CreatorId = creatorId,
            TeamId = teamId,
            CategoryId = categoryId,
            Title = title.Trim(),
            Amount = amount,
            CreatedAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc)
        };
    }

    public static AccountTransaction Restore(
        AccountTransactionId id,
        AccountId accountId,
        UserId creatorId,
        TeamId teamId,
        FinanceCategoryId? categoryId,
        string title,
        decimal amount,
        DateTime createdAt)
    {
        return new AccountTransaction
        {
            Id = id,
            AccountId = accountId,
            CreatorId = creatorId,
            TeamId = teamId,
            CategoryId = categoryId,
            Title = title,
            Amount = amount,
            CreatedAt = createdAt
        };
    }

    public void SetTitle(string title)
    {
        ValidateTitle(title);
        Title = title.Trim();
    }

    public void SetAmount(decimal amount)
    {
        ValidateAmount(amount);
        Amount = amount;
    }

    public void SetCategoryId(FinanceCategoryId? categoryId) => CategoryId = categoryId;

    public void SetCreatedAt(DateTime createdAt) =>
        CreatedAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc);

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (title.Trim().Length > 200)
            throw new ArgumentException("Title is too long (max 200).", nameof(title));
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount == 0m)
            throw new ArgumentException("Amount must be non-zero.", nameof(amount));
    }
}
