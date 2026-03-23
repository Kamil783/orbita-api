using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Services;

public interface ITodoItemService
{
    Task<Result<List<Column>>> GetWeeklyBoardAsync(Guid userId, CancellationToken ct = default);
    Task<Result> MoveAsync(Guid userId, Guid taskId, Guid fromColumnId, Guid toColumnId, int fromIndex, int toIndex, CancellationToken ct = default);
    Task<Result> MoveToAsync(Guid userId, Guid taskId, Guid targetColumnId, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid userId, Guid taskId, CancellationToken ct = default);
}
