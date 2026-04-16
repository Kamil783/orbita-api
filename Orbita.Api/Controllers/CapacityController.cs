using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.Capacity.Requests;
using Orbita.Contracts.ApiDto.Capacity.Responses;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
public class CapacityController(ICapacityService capacityService) : AuthorizedControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await capacityService.GetAsync(userId, ct);

        return res
            .Map(c => new CapacityResponse
            {
                WeekdayHours = c.WeekdayHours,
                WeekendHours = c.WeekendHours
            })
            .ToActionResult(HttpContext);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCapacityRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await capacityService.UpdateAsync(userId, request.WeekdayHours, request.WeekendHours, ct);

        return res
            .Map(c => new CapacityResponse
            {
                WeekdayHours = c.WeekdayHours,
                WeekendHours = c.WeekendHours
            })
            .ToActionResult(HttpContext);
    }
}
