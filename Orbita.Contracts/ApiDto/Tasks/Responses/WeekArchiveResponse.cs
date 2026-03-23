namespace Orbita.Contracts.ApiDto.Tasks.Responses;

public sealed class WeekArchiveResponse
{
    public string Id { get; init; } = default!;
    public string Label { get; init; } = default!;
    public string StartDate { get; init; } = default!;
    public string EndDate { get; init; } = default!;
    public List<BacklogTaskResponse> Tasks { get; init; } = [];
}
