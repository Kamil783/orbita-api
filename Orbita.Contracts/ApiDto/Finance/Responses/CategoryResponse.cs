namespace Orbita.Contracts.ApiDto.Finance.Responses;

public sealed class CategoryResponse
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Icon { get; init; } = default!;
    public string Bg { get; init; } = default!;
    public string Color { get; init; } = default!;
    public long? WeeklyLimit { get; init; }
    public long? MonthlyLimit { get; init; }
}
