namespace Orbita.Contracts.ApiDto.Wallet.Requests;

/// <summary>PATCH-семантика: null = не трогать поле.</summary>
public sealed class UpdateAccountRequest
{
    public string? Name { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal? Balance { get; set; }
}
