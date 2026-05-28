namespace Orbita.Contracts.ApiDto.Wallet.Responses;

public sealed class AccountsTotalResponse
{
    public decimal TotalRub { get; init; }
    public List<AccountTotalItemResponse> Items { get; init; } = [];
}

public sealed class AccountTotalItemResponse
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string CurrencyCode { get; init; } = default!;
    public decimal Balance { get; init; }
    /// <summary>Сколько это в рублях по последнему курсу. null — для валюты нет курса.</summary>
    public decimal? ConvertedRub { get; init; }
}
