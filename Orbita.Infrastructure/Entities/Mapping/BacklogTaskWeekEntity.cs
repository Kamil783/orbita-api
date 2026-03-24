namespace Orbita.Infrastructure.Entities.Mapping;

public class BacklogTaskWeekEntity
{
    public Guid BacklogTaskId { get; set; }
    public BacklogTaskEntity BacklogTask { get; set; } = default!;

    public Guid WeekId { get; set; }
    public WeekEntity Week { get; set; } = default!;
}
