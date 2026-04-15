using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Entities.Mapping;

namespace Orbita.Infrastructure.Extensions;

public static class WeekExtensions
{
    public static WeekEntity ToEntity(this Week week)
    {
        return new WeekEntity
        {
            Id = week.Id.Id,
            CreatorId = week.CreatorId?.Id,
            TeamId = week.TeamId.Id,
            StartDate = week.StartDate,
            EndDate = week.EndDate,
            IsArchived = week.IsArchived,
            CreatedAt = week.CreatedAt,
            BacklogTaskWeeks = week.TaskIds
                .Select(t => new BacklogTaskWeekEntity
                {
                    BacklogTaskId = t.Id,
                    WeekId = week.Id.Id
                })
                .ToList()
        };
    }

    public static Week ToDomain(this WeekEntity entity)
    {
        return Week.Restore(
            id: new WeekId(entity.Id),
            creatorId: entity.CreatorId is not null ? new UserId(entity.CreatorId.Value) : null,
            teamId: new TeamId(entity.TeamId),
            startDate: entity.StartDate,
            endDate: entity.EndDate,
            isArchived: entity.IsArchived,
            createdAt: entity.CreatedAt,
            taskIds: entity.BacklogTaskWeeks.Select(x => new BacklogTaskId(x.BacklogTaskId))
        );
    }
}
