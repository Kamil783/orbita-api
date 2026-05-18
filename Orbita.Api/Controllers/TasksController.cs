using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Helpers;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.Tasks.Requests;
using Orbita.Contracts.ApiDto.Tasks.Responses;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
public class TasksController(
    ITodoItemService service,
    IWeekService weekService,
    IRecurringTaskService recurringTaskService) : AuthorizedControllerBase
{
    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeeklyTasks(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await service.GetWeeklyBoardAsync(userId, ct);

        if (!res.IsSuccess)
            return res.ToActionResult(HttpContext);

        var currentWeek = await weekService.GetCurrentWeekAsync(userId, ct);
        var weekLabel = currentWeek is not null
            ? BacklogTaskPresentationHelper.GetWeekLabel(currentWeek.StartDate, currentWeek.EndDate)
            : null;

        return res
            .Map(columns => columns.Select(c => c.ToKanbanColumnVm(weekLabel)).ToList())
            .ToActionResult(HttpContext);
    }

    [HttpPost("move")]
    public async Task<IActionResult> MoveTask([FromBody] MoveTaskRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await service.MoveAsync(
            userId,
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
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await service.MoveToAsync(userId, request.TaskId, request.TargetStatus, ct);

        return res.ToActionResult(HttpContext);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask([FromRoute] Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await service.DeleteAsync(userId, id, ct);

        return res.ToActionResult(HttpContext);
    }

    [HttpGet("recurring")]
    public async Task<IActionResult> GetRecurringTasks(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await recurringTaskService.GetAsync(userId, ct);

        return res
            .Map(tasks => tasks.Select(ToRecurringTaskResponse).ToList())
            .ToActionResult(HttpContext);
    }

    [HttpPost("recurring")]
    public async Task<IActionResult> CreateRecurringTask(
        [FromBody] CreateRecurringTaskRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await recurringTaskService.CreateAsync(
            userId, request.Title, request.Description, request.DayOfMonth, ct);

        return res
            .Map(ToRecurringTaskResponse)
            .ToActionResult(HttpContext);
    }

    [HttpPatch("recurring/{id:guid}")]
    public async Task<IActionResult> UpdateRecurringTask(
        Guid id, [FromBody] UpdateRecurringTaskRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await recurringTaskService.UpdateAsync(
            userId,
            id,
            request.Title,
            request.Description,
            request.ClearDescription,
            request.DayOfMonth,
            request.IsCompleted,
            ct);

        return res
            .Map(ToRecurringTaskResponse)
            .ToActionResult(HttpContext);
    }

    [HttpDelete("recurring/{id:guid}")]
    public async Task<IActionResult> DeleteRecurringTask(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await recurringTaskService.DeleteAsync(userId, id, ct);
        return res.ToActionResult(HttpContext);
    }

    private static RecurringTaskResponse ToRecurringTaskResponse(Domain.Entities.RecurringTask t) =>
        new()
        {
            Id = t.Id.Id.ToString(),
            Title = t.Title,
            Description = t.Description,
            DayOfMonth = t.DayOfMonth,
            IsCompleted = t.IsCompleted,
            CreatedAt = new DateTimeOffset(t.CreatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds(),
            UpdatedAt = new DateTimeOffset(t.UpdatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds()
        };
}
