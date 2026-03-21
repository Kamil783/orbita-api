using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Commands.BacklogTasks;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Application.Services;

public class BacklogTaskService(
    IBacklogTaskRepository backlogRepository,
    ITodoItemRepository todoItemRepository,
    IColumnRepository columnRepository) : IBacklogTaskService
{
    public async Task<Result<List<BacklogTask>>> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var tasks = await backlogRepository.GetByUserAsync(userId, ct);
        return Result<List<BacklogTask>>.Ok(tasks.ToList());
    }

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

        var result = await backlogRepository.CreateAsync(task, ct);

        return Result<BacklogTask>.Ok(result);
    }

    public async Task<Result<TodoItem>> MoveToWeekAsync(Guid userId, Guid backlogTaskId, Guid targetColumnId, CancellationToken ct = default)
    {
        var backlogTask = await backlogRepository.GetAsync(backlogTaskId, ct);
        if (backlogTask is null)
            return Result<TodoItem>.NotFound("Backlog task not found.");

        if (backlogTask.CreatorId.Id != userId)
            return Result<TodoItem>.Forbidden("Access denied.");

        var column = await columnRepository.GetAsync(targetColumnId, ct);
        if (column is null)
            return Result<TodoItem>.NotFound("Target column not found.");

        var maxSort = await todoItemRepository.GetMaxSortOrderAsync(targetColumnId, ct);

        var todoItem = TodoItem.Create(
            title: backlogTask.Title,
            priority: backlogTask.Priority,
            creatorId: new UserId(userId),
            columnId: new ColumnId(targetColumnId),
            sortOrder: maxSort + 1,
            deadlineUtc: backlogTask.DueDate,
            backlogId: backlogTask.Id,
            deadlineText: backlogTask.DueDate?.ToString("dd MMM"));

        var created = await todoItemRepository.CreateAsync(todoItem, ct);

        backlogTask.SetInWeek(true);
        await backlogRepository.UpdateAsync(backlogTask, ct);

        return Result<TodoItem>.Ok(created);
    }

    public async Task<Result> RemoveFromWeekAsync(Guid userId, Guid backlogTaskId, CancellationToken ct = default)
    {
        var backlogTask = await backlogRepository.GetAsync(backlogTaskId, ct);
        if (backlogTask is null)
            return Result.NotFound("Backlog task not found.");

        if (backlogTask.CreatorId.Id != userId)
            return Result.Forbidden("Access denied.");

        var todoItem = await todoItemRepository.GetByBacklogIdAsync(backlogTaskId, ct);
        if (todoItem is not null)
            await todoItemRepository.DeleteAsync(todoItem.Id.Id, ct);

        backlogTask.SetInWeek(false);
        await backlogRepository.UpdateAsync(backlogTask, ct);

        return Result.Ok();
    }

    public async Task<Result> SetDoneAsync(Guid userId, Guid backlogTaskId, bool done, CancellationToken ct = default)
    {
        var backlogTask = await backlogRepository.GetAsync(backlogTaskId, ct);
        if (backlogTask is null)
            return Result.NotFound("Backlog task not found.");

        if (backlogTask.CreatorId.Id != userId)
            return Result.Forbidden("Access denied.");

        backlogTask.SetCompleted(done);
        await backlogRepository.UpdateAsync(backlogTask, ct);

        return Result.Ok();
    }
}
