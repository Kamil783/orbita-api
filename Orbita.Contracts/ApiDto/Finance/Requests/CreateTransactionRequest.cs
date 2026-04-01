namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class CreateTransactionRequest
{
    public required string CategoryId { get; set; }
    public required string Title { get; set; }
    public required long Amount { get; set; }
    public required bool FromBalance { get; set; }
    public DateTime? Date { get; set; }
}
