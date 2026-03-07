namespace Orbita.Application.Commands.BacklogTasks;

public class CreateBacklogTaskCommand
{
    public required string Title { get; set; }
    public required string Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public int? EstimateMinutes { get; set; }
    public int[] Assignee { get; set; }
    public string? Description { get; set; }
}
