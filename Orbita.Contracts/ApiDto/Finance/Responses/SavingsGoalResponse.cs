namespace Orbita.Contracts.ApiDto.Finance.Responses;

public sealed class SavingsGoalResponse
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public long Target { get; init; }
    public long Current { get; init; }
}
