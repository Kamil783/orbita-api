using Orbita.Domain.Enums;

namespace Orbita.Application.Models.Dto;

public record AdminBacklogTaskData(
    Guid Id,
    string Title,
    TodoItemPriority Priority,
    bool IsCompleted,
    bool InWeek,
    Guid CreatorId,
    string? CreatorName,
    IReadOnlyList<Guid> AssigneeIds,
    DateTime? DueDate,
    DateTime CreatedAt);
