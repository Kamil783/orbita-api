using Orbita.Application.Commands.BacklogTasks;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Services;

public interface IBacklogTaskService
{
    Task<Result<List<BacklogTask>>> GetAsync(Guid userId, CancellationToken ct = default);
    Task<Result<BacklogTask>> CreateAsync(Guid currentUserId, CreateBacklogTaskCommand command, CancellationToken ct = default);
    Task<Result<TodoItem>> MoveToWeekAsync(Guid userId, Guid backlogTaskId, Guid targetColumnId, CancellationToken ct = default);
    Task<Result> RemoveFromWeekAsync(Guid userId, Guid backlogTaskId, CancellationToken ct = default);
    Task<Result> SetDoneAsync(Guid userId, Guid backlogTaskId, bool done, CancellationToken ct = default);
}
