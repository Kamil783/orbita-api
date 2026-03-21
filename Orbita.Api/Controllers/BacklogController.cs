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

    [HttpPost("{backlogTaskId}/from-week")]
    public async Task<IActionResult> CreateFromWeek([FromRoute] Guid backlogTaskId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await service.CreateFromWeekAsync(userId, backlogTaskId, ct);

        return res.ToActionResult(HttpContext);
    }

    [HttpPost("{backlogTaskId}/to-week")]
    public async Task<IActionResult> MoveToWeek([FromRoute] Guid backlogTaskId, [FromBody] MoveBacklogTaskToWeekRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await service.MoveToWeekAsync(userId, backlogTaskId, request.TargetStatus, ct);

        return res.ToActionResult(HttpContext);
    }
}
