using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Entities.Mapping;
using TimeEntry = Orbita.Domain.Entities.TimeEntry;

namespace Orbita.Infrastructure.Extensions;

public static class BacklogTaskExtensions
{
    public static BacklogTaskEntity ToEntity(this BacklogTask backlogTask)
    {
        return new BacklogTaskEntity
        {
            Id = backlogTask.Id.Id,
            Title = backlogTask.Title,
            Priority = backlogTask.Priority,
            Description = backlogTask.Description,
            CreatorId = backlogTask.CreatorId.Id,
            TeamId = backlogTask.TeamId.Id,
            CreatedAt = backlogTask.CreatedAt,
            InWeek = backlogTask.InWeek,
            IsCompleted = backlogTask.IsCompleted,
            DueDate = backlogTask.DueDate,
            EstimateMinutes = backlogTask.EstimateMinutes,
            ProgressPct = backlogTask.ProgressPct,
            Assignees = backlogTask.Assignees
                .Select(a => new BacklogTaskAssigneeEntity
                {
                    BacklogTaskId = backlogTask.Id.Id,
                    UserId = a.Id
                })
                .ToList()
        };
    }

    public static BacklogTask ToDomain(this BacklogTaskEntity entity)
    {
        return BacklogTask.Restore(
             id: new BacklogTaskId(entity.Id),
             title: entity.Title,
             priority: entity.Priority,
             description: entity.Description,
             creatorId: new UserId(entity.CreatorId),
             teamId: new TeamId(entity.TeamId),
             createdAt: entity.CreatedAt,
             inWeek: entity.InWeek,
             isCompleted: entity.IsCompleted,
             dueDate: entity.DueDate,
             estimateMinutes: entity.EstimateMinutes,
             progressPct: entity.ProgressPct,
             assignees: entity.Assignees.Select(a => new UserId(a.UserId)),
             timeEntries: entity.TimeEntries.Select(t => t.ToDomain())
         );
    }
}
