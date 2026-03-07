using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Entities.Mapping;

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
            CreatedAt = backlogTask.CreatedAt,
            InWeek = backlogTask.InWeek,
            IsCompleted = backlogTask.IsCompleted,
            DueTime = backlogTask.DueTime,
            EstimateMinutes = backlogTask.EstimateMinutes,
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
             createdAt: entity.CreatedAt,
             inWeek: entity.InWeek,
             isCompleted: entity.IsCompleted,
             dueTime: entity.DueTime,
             estimateMinutes: entity.EstimateMinutes,
             assignees: entity.Assignees.Select(a => new UserId(a.UserId))
         );
    }
}
