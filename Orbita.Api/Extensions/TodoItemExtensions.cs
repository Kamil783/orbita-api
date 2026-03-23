using Orbita.Application.Helpers;
using Orbita.Contracts.ApiDto.Tasks.Responses;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;

namespace Orbita.Api.Extensions;

public static class TodoItemExtensions
{
    public static TaskCardVm ToTaskCardVm(this TodoItem item)
    {
        return new TaskCardVm
        {
            Id = item.Id.Id.ToString(),
            Title = item.Title,
            Status = item.ColumnId.Id.ToString(),
            Priority = MapPriority(item.TaskPriority),
            DeadlineText = BacklogTaskPresentationHelper.GetDueDisplayText(item.DeadlineUtc, DateTime.UtcNow),
            CompletedText = item.CompletedText,
            ProgressPct = item.ProgressPct,
            AssigneeIds = item.Assignees.Count > 0
                ? item.Assignees.Select(a => a.Id.ToString()).ToList()
                : null,
            BacklogId = item.BacklogId?.Id.ToString()
        };
    }

    public static KanbanColumnVm ToKanbanColumnVm(this Column column)
    {
        return new KanbanColumnVm
        {
            Id = column.Id.Id.ToString(),
            Title = column.Title,
            TotalCount = column.TotalCount > 0 ? column.TotalCount : column.TodoItems.Count,
            ColumnType = MapColumnType(column.Status),
            HeaderActionIcon = column.HeaderActionIcon,
            Muted = column.Muted ? true : null,
            Cards = column.TodoItems.Select(t => t.ToTaskCardVm()).ToList()
        };
    }

    private static string MapPriority(TodoItemPriority priority) => priority switch
    {
        TodoItemPriority.Critical => "critical",
        TodoItemPriority.High => "high",
        TodoItemPriority.Medium => "medium",
        TodoItemPriority.Low => "low",
        _ => "medium"
    };

    private static string MapColumnType(TodoItemStatus status) => status switch
    {
        TodoItemStatus.Todo => "todo",
        TodoItemStatus.InProgress => "inprogress",
        TodoItemStatus.Done => "done",
        TodoItemStatus.Unclassified => "custom",
        _ => "custom"
    };
}
