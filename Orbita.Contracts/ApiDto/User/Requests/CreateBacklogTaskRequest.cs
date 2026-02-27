namespace Orbita.Contracts.ApiDto.User.Requests;

public class CreateBacklogTaskRequest
{
    public string Title { get; set; }
    public string Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public int? EstimateMinutes { get; set; }
    public int? Assignee { get; set; }
    public string? Description { get; set; }
}
