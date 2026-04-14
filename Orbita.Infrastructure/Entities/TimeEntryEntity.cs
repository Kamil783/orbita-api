namespace Orbita.Infrastructure.Entities;

public class TimeEntryEntity
{
    public Guid Id { get; set; }
    public Guid BacklogTaskId { get; set; }
    public Guid UserId { get; set; }
    public int Minutes { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public BacklogTaskEntity BacklogTask { get; set; } = default!;
}
