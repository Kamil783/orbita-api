namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class UpdateSpendingLimitsRequest
{
    public required long MonthlyLimit { get; set; }
    public required long WeeklyLimit { get; set; }
}
