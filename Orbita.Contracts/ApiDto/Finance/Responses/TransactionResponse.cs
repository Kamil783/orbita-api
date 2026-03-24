namespace Orbita.Contracts.ApiDto.Finance.Responses;

public sealed class TransactionResponse
{
    public string Id { get; init; } = default!;
    public string CategoryId { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string Date { get; init; } = default!;
    public long Amount { get; init; }
    public long Timestamp { get; init; }
}
