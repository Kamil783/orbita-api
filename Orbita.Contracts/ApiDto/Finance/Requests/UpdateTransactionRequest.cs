namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class UpdateTransactionRequest
{
    public string? CategoryId { get; set; }
    public string? Title { get; set; }
    public long? Amount { get; set; }
    public bool? FromBalance { get; set; }
    public DateTime? Date { get; set; }
}
