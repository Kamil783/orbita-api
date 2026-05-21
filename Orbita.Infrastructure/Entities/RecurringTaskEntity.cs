namespace Orbita.Infrastructure.Entities;

public class RecurringTaskEntity
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public Guid TeamId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public int DayOfMonth { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? LastResetAt { get; set; }
    public DateTime? LastOverdueNotifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
