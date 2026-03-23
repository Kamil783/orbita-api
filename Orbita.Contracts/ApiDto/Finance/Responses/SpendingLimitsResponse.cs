namespace Orbita.Contracts.ApiDto.Finance.Responses;

public sealed class SpendingLimitsResponse
{
    public long MonthlyLimit { get; init; }
    public long WeeklyLimit { get; init; }
}
