namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class UpdateSavingsGoalRequest
{
    public string? Name { get; set; }
    public long? Target { get; set; }
}
