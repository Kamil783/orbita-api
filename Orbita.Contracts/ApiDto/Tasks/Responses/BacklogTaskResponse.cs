namespace Orbita.Contracts.ApiDto.Tasks.Responses;

public sealed class BacklogTaskResponse
{
    public string Id { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public string Priority { get; init; } = default!;
    public string? DueDate { get; init; }
    public string? DueDisplayText { get; init; }
    public int? EstimateMinutes { get; init; }
    public string? EstimateDisplayText { get; init; }
    public bool IsCompleted { get; init; }
    public bool InWeek { get; init; }
    public string[]? AssigneeIds { get; init; }
}
