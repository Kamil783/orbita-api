using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Commands.BacklogTasks;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Application.Services;

public class BacklogTaskService(IBacklogTaskRepository repository) : IBacklogTaskService
{
    public async Task<Result<BacklogTask>> CreateAsync(Guid currentUserId, CreateBacklogTaskCommand command, CancellationToken ct = default)
    {
        if (!Enum.TryParse<TodoItemPriority>(command.Priority, true, out var priority))
            return Result<BacklogTask>.Fail("Invalid priority.");

        var task = BacklogTask.Create(
            title: command.Title,
            priority: priority,
            description: command.Description ?? string.Empty,
            creatorId: new UserId(currentUserId),
            dueDate: command.DueDate,
            estimateMinutes: command.EstimateMinutes,
            assignees: command.AssigneeIds.Select(x => new UserId(x)));

        var result = await repository.CreateAsync(task, ct);

        return Result<BacklogTask>.Ok(result);
    }
}
