using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;

namespace Orbita.Application.Services;

public class RecurringTaskService(
    IRecurringTaskRepository recurringTaskRepository,
    ITeamProvider teamProvider) : IRecurringTaskService
{
    public async Task<Result<List<RecurringTask>>> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var tasks = await recurringTaskRepository.GetByTeamAsync(teamId, ct);
        return Result<List<RecurringTask>>.Ok(tasks);
    }

    public async Task<Result<RecurringTask>> CreateAsync(
        Guid userId,
        string title,
        string? description,
        int dayOfMonth,
        CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        RecurringTask task;
        try
        {
            task = RecurringTask.Create(
                creatorId: new UserId(userId),
                teamId: new TeamId(teamId),
                title: title,
                description: description,
                dayOfMonth: dayOfMonth);
        }
        catch (Exception ex)
        {
            return Result<RecurringTask>.Fail(ex.Message, ErrorType.Validation);
        }

        var created = await recurringTaskRepository.CreateAsync(task, ct);
        return Result<RecurringTask>.Ok(created);
    }

    public async Task<Result<RecurringTask>> UpdateAsync(
        Guid userId,
        Guid taskId,
        string? title,
        string? description,
        bool clearDescription,
        int? dayOfMonth,
        bool? isCompleted,
        CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var task = await recurringTaskRepository.GetAsync(taskId, ct);
        if (task is null)
            return Result<RecurringTask>.NotFound("Recurring task not found.");

        if (task.TeamId.Id != teamId)
            return Result<RecurringTask>.Forbidden("Access denied.");

        try
        {
            task.Update(title, description, clearDescription, dayOfMonth, isCompleted);
        }
        catch (Exception ex)
        {
            return Result<RecurringTask>.Fail(ex.Message, ErrorType.Validation);
        }

        var updated = await recurringTaskRepository.UpdateAsync(task, ct);
        return Result<RecurringTask>.Ok(updated);
    }

    public async Task<Result> DeleteAsync(Guid userId, Guid taskId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var task = await recurringTaskRepository.GetAsync(taskId, ct);
        if (task is null)
            return Result.NotFound("Recurring task not found.");

        if (task.TeamId.Id != teamId)
            return Result.Forbidden("Access denied.");

        await recurringTaskRepository.DeleteAsync(taskId, ct);
        return Result.Ok();
    }
}
