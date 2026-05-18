namespace Orbita.Contracts.ApiDto.Tasks.Requests;

public sealed class CreateRecurringTaskRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required int DayOfMonth { get; set; }
}
