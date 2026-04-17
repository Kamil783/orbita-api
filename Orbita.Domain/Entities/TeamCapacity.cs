using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class TeamCapacity
{
    private const int DefaultWeekdayHours = 8;
    private const int DefaultWeekendHours = 0;

    public TeamId TeamId { get; private set; }
    public int WeekdayHours { get; private set; }
    public int WeekendHours { get; private set; }

    private TeamCapacity() { }

    public static TeamCapacity Create(TeamId teamId)
    {
        return new TeamCapacity
        {
            TeamId = teamId,
            WeekdayHours = DefaultWeekdayHours,
            WeekendHours = DefaultWeekendHours
        };
    }

    public static TeamCapacity Restore(TeamId teamId, int weekdayHours, int weekendHours)
    {
        return new TeamCapacity
        {
            TeamId = teamId,
            WeekdayHours = weekdayHours,
            WeekendHours = weekendHours
        };
    }

    public void Update(int weekdayHours, int weekendHours)
    {
        WeekdayHours = weekdayHours;
        WeekendHours = weekendHours;
    }
}
