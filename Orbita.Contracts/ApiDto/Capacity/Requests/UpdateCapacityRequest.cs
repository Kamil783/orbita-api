namespace Orbita.Contracts.ApiDto.Capacity.Requests;

public sealed class UpdateCapacityRequest
{
    public required double WeekdayHours { get; set; }
    public required double WeekendHours { get; set; }
}
