using Orbita.Infrastructure.Entities.Mapping;

namespace Orbita.Infrastructure.Entities;

public class WeekEntity
{
    public Guid Id { get; set; }
    public Guid? CreatorId { get; set; }
    public Guid TeamId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<BacklogTaskWeekEntity> BacklogTaskWeeks { get; set; } = [];
}
