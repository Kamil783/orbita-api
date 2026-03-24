namespace Orbita.Contracts.ApiDto.Tasks.Responses;

public sealed class TaskCardVm
{
    public string Id { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string Status { get; init; } = default!;
    public string Priority { get; init; } = default!;
    public string? DeadlineText { get; init; }
    public string? CompletedText { get; init; }
    public int? ProgressPct { get; init; }
    public string[]? AssigneeIds { get; init; }
    public string? BacklogId { get; init; }
    public string? WeekLabel { get; init; }
}
