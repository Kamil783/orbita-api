namespace Orbita.Contracts.ApiDto.Tasks.Requests;

public sealed class UpdateRecurringTaskRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    /// <summary>Если true — описание сбрасывается в null, Description игнорируется.</summary>
    public bool ClearDescription { get; set; }
    public int? DayOfMonth { get; set; }
    public bool? IsCompleted { get; set; }
}
