using Orbita.Application.Commands.BacklogTasks;
using Orbita.Application.Helpers;
using Orbita.Contracts.ApiDto.Tasks.Requests;
using Orbita.Contracts.ApiDto.Tasks.Responses;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;

namespace Orbita.Api.Extensions;

public static class BacklogTaskExtensions
{
    public static CreateBacklogTaskCommand ToCommand(this CreateBacklogTaskRequest request)
    {
        return new CreateBacklogTaskCommand
        {
            Title = request.Title,
            Priority = request.Priority,
            DueDate = request.DueDate,
            EstimateMinutes = request.EstimateMinutes,
            AssigneeIds = request.Assignee,
            Description = request.Description,
            ProgressPct = request.ProgressPct
        };
    }

    public static UpdateBacklogTaskCommand ToCommand(this UpdateBacklogTaskRequest request)
    {
        return new UpdateBacklogTaskCommand
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate,
            EstimateMinutes = request.EstimateMinutes,
            AssigneeIds = request.AssigneeIds,
            ProgressPct = request.ProgressPct
        };
    }

    public static BacklogTaskResponse ToResponse(this BacklogTask task, DateTime now, string[]? weekLabels = null)
    {
        return new BacklogTaskResponse
        {
            Id = task.Id.Id.ToString(),
            Title = task.Title,
            Description = string.IsNullOrWhiteSpace(task.Description) ? null : task.Description,
            Priority = MapPriority(task.Priority),
            DueDate = task.DueDate?.ToString("yyyy-MM-dd"),
            DueDisplayText = BacklogTaskPresentationHelper.GetDueDisplayText(task.DueDate, now),
            EstimateMinutes = task.EstimateMinutes,
            EstimateDisplayText = BacklogTaskPresentationHelper.GetEstimateDisplayText(task.EstimateMinutes),
            IsCompleted = task.IsCompleted,
            InWeek = task.InWeek,
            ProgressPct = task.ProgressPct,
            AssigneeIds = task.Assignees.Select(x => x.Id.ToString()).ToArray(),
            WeekLabels = weekLabels
        };
    }

    private static string MapPriority(TodoItemPriority priority) => priority switch
    {
        TodoItemPriority.Critical => "critical",
        TodoItemPriority.High => "high",
        TodoItemPriority.Medium => "medium",
        TodoItemPriority.Low => "low",
        _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, null)
    };
}
