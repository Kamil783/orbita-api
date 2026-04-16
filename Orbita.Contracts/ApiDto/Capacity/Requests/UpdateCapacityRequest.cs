namespace Orbita.Contracts.ApiDto.Capacity.Requests;

public sealed class UpdateCapacityRequest
{
    public required int WeekdayHours { get; set; }
    public required int WeekendHours { get; set; }
}
