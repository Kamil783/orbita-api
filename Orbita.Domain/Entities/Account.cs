using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

/// <summary>
/// Дополнительный счёт пользователя в произвольной валюте. Принадлежит команде
/// (как и FinanceBalance), но создатель фиксируется отдельно. Баланс хранится как
/// decimal(28,8) — хватает и для фиата (до 2 знаков), и для крипты (до 8).
/// </summary>
public class Account
{
    public AccountId Id { get; private set; } = default!;
    public UserId CreatorId { get; private set; } = default!;
    public TeamId TeamId { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string CurrencyCode { get; private set; } = default!;
    public decimal Balance { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Account() { }

    public static Account Create(
        UserId creatorId, TeamId teamId, string name, string currencyCode, decimal balance)
    {
        ValidateName(name);
        ValidateCurrencyCode(currencyCode);

        var now = DateTime.UtcNow;
        return new Account
        {
            Id = new AccountId(Guid.NewGuid()),
            CreatorId = creatorId,
            TeamId = teamId,
            Name = name.Trim(),
            CurrencyCode = currencyCode.Trim().ToUpperInvariant(),
            Balance = balance,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public static Account Restore(
        AccountId id, UserId creatorId, TeamId teamId,
        string name, string currencyCode, decimal balance,
        DateTime createdAt, DateTime updatedAt)
    {
        return new Account
        {
            Id = id,
            CreatorId = creatorId,
            TeamId = teamId,
            Name = name,
            CurrencyCode = currencyCode,
            Balance = balance,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public void Update(string? name, string? currencyCode, decimal? balance)
    {
        if (name is not null)
        {
            ValidateName(name);
            Name = name.Trim();
        }

        if (currencyCode is not null)
        {
            ValidateCurrencyCode(currencyCode);
            CurrencyCode = currencyCode.Trim().ToUpperInvariant();
        }

        if (balance.HasValue)
            Balance = balance.Value;

        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (name.Trim().Length > 100)
            throw new ArgumentException("Name is too long (max 100).", nameof(name));
    }

    private static void ValidateCurrencyCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Currency code is required.", nameof(code));
        if (code.Trim().Length is < 1 or > 10)
            throw new ArgumentException("Currency code length must be 1..10.", nameof(code));
    }
}
