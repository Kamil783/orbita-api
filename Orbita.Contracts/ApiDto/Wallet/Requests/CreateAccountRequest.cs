namespace Orbita.Contracts.ApiDto.Wallet.Requests;

public sealed class CreateAccountRequest
{
    public required string Name { get; set; }
    public required string CurrencyCode { get; set; }
    public required decimal Balance { get; set; }
}
