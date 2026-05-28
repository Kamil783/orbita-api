namespace Orbita.Contracts.ApiDto.Wallet.Responses;

public sealed class AccountResponse
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string CurrencyCode { get; init; } = default!;
    public decimal Balance { get; init; }
    public long CreatedAt { get; init; }
    public long UpdatedAt { get; init; }
}
