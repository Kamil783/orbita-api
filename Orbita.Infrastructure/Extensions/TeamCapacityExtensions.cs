using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class TeamCapacityExtensions
{
    public static TeamCapacityEntity ToEntity(this TeamCapacity capacity)
    {
        return new TeamCapacityEntity
        {
            TeamId = capacity.TeamId.Id,
            WeekdayHours = capacity.WeekdayHours,
            WeekendHours = capacity.WeekendHours
        };
    }

    public static TeamCapacity ToDomain(this TeamCapacityEntity entity)
    {
        return TeamCapacity.Restore(
            teamId: new TeamId(entity.TeamId),
            weekdayHours: entity.WeekdayHours,
            weekendHours: entity.WeekendHours
        );
    }
}
