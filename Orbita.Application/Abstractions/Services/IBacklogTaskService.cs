using Orbita.Application.Commands.BacklogTasks;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Services;

public interface IBacklogTaskService
{
    Task<Result<List<BacklogTask>>> GetAsync(Guid userId, CancellationToken ct = default);
    Task<Result<BacklogTask>> CreateAsync(Guid currentUserId, CreateBacklogTaskCommand command, CancellationToken ct = default);
    Task<Result<BacklogTask>> UpdateAsync(Guid userId, Guid backlogTaskId, UpdateBacklogTaskCommand command, CancellationToken ct = default);
    Task<Result<TodoItem>> MoveToWeekAsync(Guid userId, Guid backlogTaskId, Guid targetColumnId, CancellationToken ct = default);
    Task<Result> RemoveFromWeekAsync(Guid userId, Guid backlogTaskId, CancellationToken ct = default);
    Task<Result> SetDoneAsync(Guid userId, Guid backlogTaskId, bool done, CancellationToken ct = default);
    Task<Result<TimeEntry>> AddTimeEntryAsync(Guid userId, Guid backlogTaskId, int minutes, string? description, CancellationToken ct = default);
    Task<Result> DeleteTimeEntryAsync(Guid userId, Guid backlogTaskId, Guid entryId, CancellationToken ct = default);
}
