namespace Orbita.Contracts.ApiDto.Wallet.Responses;

public sealed class AccountTransactionResponse
{
    public string Id { get; init; } = default!;
    public string AccountId { get; init; } = default!;
    public string? CategoryId { get; init; }
    public string Title { get; init; } = default!;
    /// <summary>Знаковая сумма в валюте счёта.</summary>
    public decimal Amount { get; init; }
    /// <summary>"YYYY-MM-DD" в UTC.</summary>
    public string Date { get; init; } = default!;
    public long Timestamp { get; init; }
}
