namespace Orbita.Infrastructure.Entities;

public class TeamCapacityEntity
{
    public Guid TeamId { get; set; }
    public int WeekdayHours { get; set; }
    public int WeekendHours { get; set; }
}
