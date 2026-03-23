namespace Orbita.Contracts.ApiDto.Finance.Responses;

public sealed class ChartDataPointResponse
{
    public string Label { get; init; } = default!;
    public decimal Value { get; init; }
}
