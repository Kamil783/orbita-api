using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.Tasks.Requests;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
public class BacklogController(IBacklogTaskService service) : AuthorizedControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBacklog(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await service.GetAsync(userId, ct);

        var now = DateTime.UtcNow;

        return res
            .Map(tasks => tasks.Select(x => x.ToResponse(now)).ToList())
            .ToActionResult(HttpContext);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBacklog(CreateBacklogTaskRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await service.CreateAsync(userId, request.ToCommand(), ct);

        var now = DateTime.UtcNow;

        return res
            .Map(task => task.ToResponse(now))
            .ToActionResult(HttpContext);
    }

    [HttpPatch("{backlogTaskId}")]
    public async Task<IActionResult> UpdateBacklog([FromRoute] Guid backlogTaskId, [FromBody] UpdateBacklogTaskRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await service.UpdateAsync(userId, backlogTaskId, request.ToCommand(), ct);

        var now = DateTime.UtcNow;

        return res
            .Map(task => task.ToResponse(now))
            .ToActionResult(HttpContext);
    }

    [HttpPost("{backlogTaskId}/to-week")]
    public async Task<IActionResult> MoveToWeek([FromRoute] Guid backlogTaskId, [FromBody] MoveBacklogTaskToWeekRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await service.MoveToWeekAsync(userId, backlogTaskId, request.TargetStatus, ct);

        return res
            .Map(item => new { kanbanCard = item.ToTaskCardVm() })
            .ToActionResult(HttpContext);
    }

    [HttpPost("{backlogTaskId}/from-week")]
    public async Task<IActionResult> RemoveFromWeek([FromRoute] Guid backlogTaskId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await service.RemoveFromWeekAsync(userId, backlogTaskId, ct);

        return res.ToActionResult(HttpContext);
    }

    [HttpPatch("{backlogTaskId}/done")]
    public async Task<IActionResult> SetDone([FromRoute] Guid backlogTaskId, [FromBody] SetBacklogDoneRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await service.SetDoneAsync(userId, backlogTaskId, request.Done, ct);

        return res.ToActionResult(HttpContext);
    }
}
