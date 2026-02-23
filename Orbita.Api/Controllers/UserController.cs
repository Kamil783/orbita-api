using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using System.Security.Claims;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService service) : ControllerBase
{
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized(); 

        var result = await service.GetDataAsync(userId, ct);

        return result.ToActionResult(HttpContext);
    }
}
