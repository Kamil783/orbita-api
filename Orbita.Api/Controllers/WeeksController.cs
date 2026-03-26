using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Helpers;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.Tasks.Requests;
using Orbita.Contracts.ApiDto.Tasks.Responses;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
public class WeeksController(IWeekService weekService) : AuthorizedControllerBase
{
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentWeek(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var week = await weekService.GetCurrentWeekAsync(userId, ct);
        if (week is null)
            return NotFound();

        var response = new WeekResponse
        {
            StartDate = week.StartDate.ToString("yyyy-MM-dd"),
            EndDate = week.EndDate.ToString("yyyy-MM-dd")
        };

        return Result<WeekResponse>.Ok(response).ToActionResult(HttpContext);
    }

    [HttpPost("new")]
    public async Task<IActionResult> CreateNewWeek([FromBody] CreateWeekRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await weekService.CreateNewWeekAsync(userId, request.StartDate, request.EndDate, ct);

        return res
            .Map(w => new WeekResponse
            {
                StartDate = w.StartDate.ToString("yyyy-MM-dd"),
                EndDate = w.EndDate.ToString("yyyy-MM-dd")
            })
            .ToActionResult(HttpContext);
    }

    [HttpGet("archives")]
    public async Task<IActionResult> GetArchives(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await weekService.GetArchivesAsync(userId, ct);

        if (!res.IsSuccess)
            return res.ToActionResult(HttpContext);

        var now = DateTime.UtcNow;
        var archives = res.Value!.Select(entry => new WeekArchiveResponse
        {
            Id = entry.Week.Id.Id.ToString(),
            Label = BacklogTaskPresentationHelper.GetWeekLabel(entry.Week.StartDate, entry.Week.EndDate),
            StartDate = entry.Week.StartDate.ToString("yyyy-MM-dd"),
            EndDate = entry.Week.EndDate.ToString("yyyy-MM-dd"),
            Tasks = entry.Tasks.Select(t =>
            {
                var weeks = new[] { BacklogTaskPresentationHelper.GetWeekLabel(entry.Week.StartDate, entry.Week.EndDate) };
                return t.ToResponse(now, weeks);
            }).ToList()
        }).ToList();

        return Result<List<WeekArchiveResponse>>.Ok(archives).ToActionResult(HttpContext);
    }
}
