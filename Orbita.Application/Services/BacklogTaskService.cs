using Orbita.Application.Abstractions;
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
    IColumnRepository columnRepository,
    IWeekRepository weekRepository,
    ITeamProvider teamProvider,
    INotificationDispatcher notificationDispatcher,
    IUnitOfWork unitOfWork) : IBacklogTaskService
{
    public async Task<Result<List<BacklogTask>>> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var tasks = await backlogRepository.GetByTeamAsync(teamId, ct);
        return Result<List<BacklogTask>>.Ok(tasks.ToList());
    }

    public async Task<Result<BacklogTask>> CreateAsync(Guid currentUserId, CreateBacklogTaskCommand command, CancellationToken ct = default)
    {
        if (!Enum.TryParse<TodoItemPriority>(command.Priority, true, out var priority))
            return Result<BacklogTask>.Fail("Invalid priority.");

        var teamId = await teamProvider.GetTeamIdAsync(currentUserId, ct);

        var task = BacklogTask.Create(
            title: command.Title,
            priority: priority,
            description: command.Description ?? string.Empty,
            creatorId: new UserId(currentUserId),
            teamId: new TeamId(teamId),
            dueDate: command.DueDate,
            estimateMinutes: command.EstimateMinutes,
            progressPct: command.ProgressPct,
            assignees: command.AssigneeIds.Select(x => new UserId(x)));

        var result = await backlogRepository.CreateAsync(task, ct);

        // Уведомляем всех назначенных, кроме самого инициатора.
        var newAssignees = command.AssigneeIds
            .Where(id => id != currentUserId)
            .Distinct()
            .ToList();

        await NotifyAssignedAsync(newAssignees, result, ct);

        return Result<BacklogTask>.Ok(result);
    }

    public async Task<Result<BacklogTask>> UpdateAsync(Guid userId, Guid backlogTaskId, UpdateBacklogTaskCommand command, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var backlogTask = await backlogRepository.GetAsync(backlogTaskId, ct);
        if (backlogTask is null)
            return Result<BacklogTask>.NotFound("Backlog task not found.");

        if (backlogTask.TeamId.Id != teamId)
            return Result<BacklogTask>.Forbidden("Access denied.");

        if (command.Title is not null)
            backlogTask.SetTitle(command.Title);

        if (command.Description is not null)
            backlogTask.SetDescription(command.Description);

        if (command.Priority is not null)
        {
            if (!Enum.TryParse<TodoItemPriority>(command.Priority, true, out var priority))
                return Result<BacklogTask>.Fail("Invalid priority.");

            backlogTask.SetPriority(priority);
        }

        if (command.DueDate.HasValue)
            backlogTask.SetDueDate(command.DueDate);

        if (command.EstimateMinutes.HasValue)
            backlogTask.SetEstimateMinutes(command.EstimateMinutes);

        if (command.ProgressPct.HasValue)
            backlogTask.SetProgressPct(command.ProgressPct);

        // Запоминаем старых ассайни до изменения, чтобы посчитать дельту для уведомлений.
        var previousAssignees = backlogTask.Assignees.Select(a => a.Id).ToHashSet();

        List<Guid> newlyAssigned = [];
        if (command.AssigneeIds is not null)
        {
            var nextAssignees = command.AssigneeIds.Distinct().ToList();
            backlogTask.SetAssignees(nextAssignees.Select(x => new UserId(x)));

            newlyAssigned = nextAssignees
                .Where(id => !previousAssignees.Contains(id) && id != userId)
                .ToList();
        }

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var todoItem = await todoItemRepository.GetByBacklogIdAsync(backlogTask.Id.Id, ct);

            if (todoItem is not null)
            {
                todoItem.SyncFromBacklog(backlogTask);
                await todoItemRepository.UpdateAsync(todoItem, ct);
            }

            var updated = await backlogRepository.UpdateAsync(backlogTask, ct);

            await NotifyAssignedAsync(newlyAssigned, updated!, ct);

            await unitOfWork.CommitAsync(ct);

            return Result<BacklogTask>.Ok(updated!);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    private async Task NotifyAssignedAsync(IReadOnlyCollection<Guid> recipients, BacklogTask task, CancellationToken ct)
    {
        if (recipients.Count == 0)
            return;

        var message = task.DueDate.HasValue
            ? $"Вам назначена задача «{task.Title}» (срок: {task.DueDate.Value:dd.MM.yyyy})."
            : $"Вам назначена задача «{task.Title}».";

        foreach (var userId in recipients)
        {
            await notificationDispatcher.SendAsync(
                userId,
                NotificationType.Task,
                title: "Новая задача",
                message: message,
                pushOverHub: true,
                ct);
        }
    }

    public async Task<Result<TodoItem>> MoveToWeekAsync(Guid userId, Guid backlogTaskId, Guid targetColumnId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var backlogTask = await backlogRepository.GetAsync(backlogTaskId, ct);
        if (backlogTask is null)
            return Result<TodoItem>.NotFound("Backlog task not found.");

        if (backlogTask.TeamId.Id != teamId)
            return Result<TodoItem>.Forbidden("Access denied.");

        var column = await columnRepository.GetAsync(targetColumnId, ct);
        if (column is null)
            return Result<TodoItem>.NotFound("Target column not found.");

        var maxSort = await todoItemRepository.GetMaxSortOrderAsync(targetColumnId, ct);

        var todoItem = TodoItem.Create(
            title: backlogTask.Title,
            priority: backlogTask.Priority,
            creatorId: new UserId(userId),
            teamId: new TeamId(teamId),
            columnId: new ColumnId(targetColumnId),
            sortOrder: maxSort + 1,
            deadlineUtc: backlogTask.DueDate,
            progressPct: backlogTask.TrackProgress ? 0 : null,
            backlogId: backlogTask.Id,
            assignees: backlogTask.Assignees);

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var created = await todoItemRepository.CreateAsync(todoItem, ct);

            backlogTask.SetInWeek(true);
            await backlogRepository.UpdateAsync(backlogTask, ct);

            var currentWeek = await weekRepository.GetCurrentAsync(teamId, ct);
            if (currentWeek is not null)
            {
                currentWeek.AddTask(backlogTask.Id);
                await weekRepository.UpdateAsync(currentWeek, ct);
            }

            await unitOfWork.CommitAsync(ct);

            return Result<TodoItem>.Ok(created);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<Result> RemoveFromWeekAsync(Guid userId, Guid backlogTaskId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var backlogTask = await backlogRepository.GetAsync(backlogTaskId, ct);
        if (backlogTask is null)
            return Result.NotFound("Backlog task not found.");

        if (backlogTask.TeamId.Id != teamId)
            return Result.Forbidden("Access denied.");

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var todoItem = await todoItemRepository.GetByBacklogIdAsync(backlogTaskId, ct);
            if (todoItem is not null)
                await todoItemRepository.DeleteAsync(todoItem.Id.Id, ct);

            backlogTask.SetInWeek(false);
            await backlogRepository.UpdateAsync(backlogTask, ct);

            var currentWeek = await weekRepository.GetCurrentAsync(teamId, ct);
            if (currentWeek is not null)
            {
                currentWeek.RemoveTask(backlogTask.Id);
                await weekRepository.UpdateAsync(currentWeek, ct);
            }

            await unitOfWork.CommitAsync(ct);

            return Result.Ok();
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<Result> SetDoneAsync(Guid userId, Guid backlogTaskId, bool done, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var backlogTask = await backlogRepository.GetAsync(backlogTaskId, ct);
        if (backlogTask is null)
            return Result.NotFound("Backlog task not found.");

        if (backlogTask.TeamId.Id != teamId)
            return Result.Forbidden("Access denied.");

        backlogTask.SetCompleted(done);
        await backlogRepository.UpdateAsync(backlogTask, ct);

        return Result.Ok();
    }

    public async Task<Result<TimeEntry>> AddTimeEntryAsync(Guid userId, Guid backlogTaskId, int minutes, string? description, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var backlogTask = await backlogRepository.GetAsync(backlogTaskId, ct);
        if (backlogTask is null)
            return Result<TimeEntry>.NotFound("Backlog task not found.");

        if (backlogTask.TeamId.Id != teamId)
            return Result<TimeEntry>.Forbidden("Access denied.");

        var entry = TimeEntry.Create(
            backlogTaskId: new BacklogTaskId(backlogTaskId),
            userId: new UserId(userId),
            minutes: minutes,
            description: description);

        var created = await backlogRepository.AddTimeEntryAsync(entry, ct);
        return Result<TimeEntry>.Ok(created);
    }

    public async Task<Result> DeleteTimeEntryAsync(Guid userId, Guid backlogTaskId, Guid entryId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var backlogTask = await backlogRepository.GetAsync(backlogTaskId, ct);
        if (backlogTask is null)
            return Result.NotFound("Backlog task not found.");

        if (backlogTask.TeamId.Id != teamId)
            return Result.Forbidden("Access denied.");

        var deleted = await backlogRepository.DeleteTimeEntryAsync(entryId, backlogTaskId, ct);
        if (!deleted)
            return Result.NotFound("Time entry not found.");

        return Result.Ok();
    }
}
