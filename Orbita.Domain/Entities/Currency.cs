using Orbita.Domain.Enums;

namespace Orbita.Domain.Entities;

/// <summary>
/// Валюта. Code (ISO 4217 или произвольный тикер для крипты) — первичный ключ.
/// Хранит самый свежий курс к рублю, чтобы не плодить отдельную таблицу истории
/// (если позже понадобится — добавим CurrencyRateHistory отдельно).
/// </summary>
public class Currency
{
    /// <summary>ISO 4217 (USD, EUR) или тикер крипты (BTC, USDT). 1..10 символов.</summary>
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    /// <summary>ISO numeric code, если применимо (CBR его отдаёт). Для крипты — null.</summary>
    public int? NumCode { get; private set; }
    public CurrencyKind Kind { get; private set; }

    /// <summary>Курс «<see cref="Nominal"/> единиц этой валюты = RateToRub рублей». Для RUB — 1.</summary>
    public decimal? RateToRub { get; private set; }
    public int Nominal { get; private set; }
    public DateTime? RateFetchedAt { get; private set; }

    private Currency() { }

    public static Currency Create(string code, string name, int? numCode, CurrencyKind kind)
    {
        ValidateCode(code);
        ValidateName(name);

        return new Currency
        {
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            NumCode = numCode,
            Kind = kind,
            RateToRub = null,
            Nominal = 1,
            RateFetchedAt = null
        };
    }

    public static Currency Restore(
        string code, string name, int? numCode, CurrencyKind kind,
        decimal? rateToRub, int nominal, DateTime? rateFetchedAt)
    {
        return new Currency
        {
            Code = code,
            Name = name,
            NumCode = numCode,
            Kind = kind,
            RateToRub = rateToRub,
            Nominal = nominal,
            RateFetchedAt = rateFetchedAt
        };
    }

    public void UpdateRate(decimal rateToRub, int nominal, DateTime fetchedAt)
    {
        if (rateToRub <= 0)
            throw new ArgumentOutOfRangeException(nameof(rateToRub), "Rate must be positive.");
        if (nominal <= 0)
            throw new ArgumentOutOfRangeException(nameof(nominal), "Nominal must be positive.");

        RateToRub = rateToRub;
        Nominal = nominal;
        RateFetchedAt = DateTime.SpecifyKind(fetchedAt, DateTimeKind.Utc);
    }

    public void UpdateMetadata(string name, int? numCode, CurrencyKind kind)
    {
        ValidateName(name);
        Name = name.Trim();
        NumCode = numCode;
        Kind = kind;
    }

    /// <summary>Пересчитывает баланс в этой валюте в рубли. Возвращает null, если курс не известен.</summary>
    public decimal? ConvertToRub(decimal balance)
    {
        if (Code == "RUB") return balance;
        if (RateToRub is null || Nominal <= 0) return null;
        return balance * RateToRub.Value / Nominal;
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));
        var trimmed = code.Trim();
        if (trimmed.Length is < 1 or > 10)
            throw new ArgumentException("Code length must be 1..10.", nameof(code));
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (name.Trim().Length > 100)
            throw new ArgumentException("Name is too long (max 100).", nameof(name));
    }
}
