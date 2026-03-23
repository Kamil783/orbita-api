namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class CreateSavingsGoalRequest
{
    public required string Name { get; set; }
    public required long Target { get; set; }
}
