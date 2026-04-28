namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class CreatePlannedPurchaseRequest
{
    public required string Title { get; set; }
    /// <summary>ISO date "YYYY-MM-DD".</summary>
    public required string Date { get; set; }
    public required long Amount { get; set; }
    public Guid? AssigneeId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Note { get; set; }
}
