using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class TimeEntryExtensions
{
    public static TimeEntryEntity ToEntity(this TimeEntry e)
    {
        return new TimeEntryEntity
        {
            Id = e.Id.Id,
            BacklogTaskId = e.BacklogTaskId.Id,
            UserId = e.UserId.Id,
            Minutes = e.Minutes,
            Description = e.Description,
            CreatedAt = e.CreatedAt
        };
    }

    public static TimeEntry ToDomain(this TimeEntryEntity e)
    {
        return TimeEntry.Restore(
            id: new TimeEntryId(e.Id),
            backlogTaskId: new BacklogTaskId(e.BacklogTaskId),
            userId: new UserId(e.UserId),
            minutes: e.Minutes,
            description: e.Description,
            createdAt: e.CreatedAt
        );
    }
}
