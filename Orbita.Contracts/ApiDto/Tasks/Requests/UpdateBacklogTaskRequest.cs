namespace Orbita.Contracts.ApiDto.Tasks.Requests;

public sealed class UpdateBacklogTaskRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public int? EstimateMinutes { get; set; }
    public Guid[]? AssigneeIds { get; set; }
    public int? ProgressPct { get; set; }
}
