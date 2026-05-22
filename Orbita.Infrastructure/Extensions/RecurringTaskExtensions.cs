using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class RecurringTaskExtensions
{
    public static RecurringTaskEntity ToEntity(this RecurringTask task)
    {
        return new RecurringTaskEntity
        {
            Id = task.Id.Id,
            CreatorId = task.CreatorId.Id,
            TeamId = task.TeamId.Id,
            Title = task.Title,
            Description = task.Description,
            DayOfMonth = task.DayOfMonth,
            IsCompleted = task.IsCompleted,
            LastResetAt = task.LastResetAt,
            LastOverdueNotifiedAt = task.LastOverdueNotifiedAt,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }

    public static RecurringTask ToDomain(this RecurringTaskEntity e)
    {
        return RecurringTask.Restore(
            id: new RecurringTaskId(e.Id),
            creatorId: new UserId(e.CreatorId),
            teamId: new TeamId(e.TeamId),
            title: e.Title,
            description: e.Description,
            dayOfMonth: e.DayOfMonth,
            isCompleted: e.IsCompleted,
            lastResetAt: e.LastResetAt,
            lastOverdueNotifiedAt: e.LastOverdueNotifiedAt,
            createdAt: e.CreatedAt,
            updatedAt: e.UpdatedAt);
    }
}
