namespace Orbita.Contracts.ApiDto.Tasks.Requests;

public sealed class CreateBacklogTaskRequest
{
    public required string Title { get; set; }
    public required string Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public int? EstimateMinutes { get; set; }
    public Guid[] Assignee { get; set; } = [];
    public string? Description { get; set; }
    public int? ProgressPct { get; set; }
}
