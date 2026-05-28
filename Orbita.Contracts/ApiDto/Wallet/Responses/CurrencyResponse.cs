namespace Orbita.Contracts.ApiDto.Wallet.Responses;

public sealed class CurrencyResponse
{
    public string Code { get; init; } = default!;
    public string Name { get; init; } = default!;
    public int? NumCode { get; init; }
    /// <summary>"fiat" | "crypto".</summary>
    public string Kind { get; init; } = default!;
    /// <summary>Сколько рублей за <see cref="Nominal"/> единиц. null — курс ещё не получали.</summary>
    public decimal? RateToRub { get; init; }
    public int Nominal { get; init; }
    /// <summary>Unix ms; null если курс ещё не получали.</summary>
    public long? RateFetchedAt { get; init; }
}
