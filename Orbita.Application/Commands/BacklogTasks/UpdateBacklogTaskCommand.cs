namespace Orbita.Application.Commands.BacklogTasks;

public class UpdateBacklogTaskCommand
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Priority { get; init; }
    public DateTime? DueDate { get; init; }
    public int? EstimateMinutes { get; init; }
    public IReadOnlyCollection<Guid>? AssigneeIds { get; init; }
    public int? ProgressPct { get; init; }
}
