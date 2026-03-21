using Orbita.Application.Commands.BacklogTasks;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Services;

public interface IBacklogTaskService
{
    Task<Result<List<BacklogTask>>> GetAsync(Guid userId, CancellationToken ct = default);
    Task<Result<BacklogTask>> CreateAsync(Guid currentUserId, CreateBacklogTaskCommand command, CancellationToken ct = default);
    Task<Result> AddToBoardAsync(Guid userId, Guid taskId, CancellationToken ct = default);
    Task<Result> RemoveFromBoardAsync(Guid userId, Guid taskId, CancellationToken ct = default);
    Task<Result> UpdateStatusAsync(Guid userId, Guid taskId, CancellationToken ct = default);
}
