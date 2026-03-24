namespace Orbita.Contracts.ApiDto.Tasks.Requests;

public sealed class CreateWeekRequest
{
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
}
