namespace Orbita.Contracts.ApiDto.Tasks.Requests;

public sealed class AddTimeEntryRequest
{
    public required int Minutes { get; set; }
    public string? Description { get; set; }
}
