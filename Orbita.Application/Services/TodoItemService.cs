using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Services;

public class TodoItemService(
    ITodoItemRepository todoItemRepository,
    IColumnRepository columnRepository) : ITodoItemService
{
    public async Task<Result<List<Column>>> GetWeeklyBoardAsync(Guid userId, CancellationToken ct = default)
    {
        var columns = await columnRepository.GetAllWithItemsAsync(userId, ct);
        return Result<List<Column>>.Ok(columns.ToList());
    }

    public async Task<Result> MoveAsync(
        Guid taskId, Guid fromColumnId, Guid toColumnId,
        int fromIndex, int toIndex, CancellationToken ct = default)
    {
        var success = await todoItemRepository.MoveCardAsync(taskId, fromColumnId, toColumnId, fromIndex, toIndex, ct);

        if (!success)
            return Result.Conflict("Task at the specified position does not match the expected task.");

        return Result.Ok();
    }

    public async Task<Result> MoveToAsync(Guid taskId, Guid targetColumnId, CancellationToken ct = default)
    {
        var item = await todoItemRepository.GetAsync(taskId, ct);
        if (item is null)
            return Result.NotFound("Task not found.");

        var column = await columnRepository.GetAsync(targetColumnId, ct);
        if (column is null)
            return Result.NotFound("Target column not found.");

        await todoItemRepository.MoveCardToEndAsync(taskId, targetColumnId, ct);
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid taskId, CancellationToken ct = default)
    {
        var item = await todoItemRepository.GetAsync(taskId, ct);
        if (item is null)
            return Result.NotFound("Task not found.");

        await todoItemRepository.DeleteAsync(taskId, ct);
        return Result.Ok();
    }
}
