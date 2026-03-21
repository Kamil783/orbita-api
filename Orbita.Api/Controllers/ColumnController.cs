using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.Tasks.Requests;

namespace Orbita.Api.Controllers;

[Route("api/Columns")]
public class ColumnController(IColumnService service) : AuthorizedControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateColumn([FromBody] CreateColumnRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out _))
            return Unauthorized();

        var res = await service.CreateAsync(request.Title, ct);

        return res
            .Map(column => new { id = column.Id.Id.ToString() })
            .ToActionResult(HttpContext);
    }
}
