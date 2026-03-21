using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.Tasks.Requests;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
public class TasksController(ITodoItemService service) : AuthorizedControllerBase
{
    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeeklyTasks(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await service.GetWeeklyBoardAsync(userId, ct);

        return res
            .Map(columns => columns.Select(c => c.ToKanbanColumnVm()).ToList())
            .ToActionResult(HttpContext);
    }

    [HttpPost("move")]
    public async Task<IActionResult> MoveTask([FromBody] MoveTaskRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out _))
            return Unauthorized();

        var res = await service.MoveAsync(
            request.TaskId,
            request.FromColumnId,
            request.ToColumnId,
            request.FromIndex,
            request.ToIndex,
            ct);

        return res.ToActionResult(HttpContext);
    }

    [HttpPost("move-to")]
    public async Task<IActionResult> MoveTaskTo([FromBody] MoveTaskToRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out _))
            return Unauthorized();

        var res = await service.MoveToAsync(request.TaskId, request.TargetStatus, ct);

        return res.ToActionResult(HttpContext);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask([FromRoute] Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out _))
            return Unauthorized();

        var res = await service.DeleteAsync(id, ct);

        return res.ToActionResult(HttpContext);
    }
}
