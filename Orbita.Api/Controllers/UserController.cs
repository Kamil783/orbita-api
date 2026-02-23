using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using System.Security.Claims;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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

    [HttpPatch("profile")]
    public async Task<IActionResult> ChangeProfile(CancellationToken ct)
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var result = await service.GetDataAsync(userId, ct);

        return result.ToActionResult(HttpContext);
    }

    [HttpPut("avatar")]
    public async Task<IActionResult> ChangeAvatar([FromForm] IFormFile file, CancellationToken ct)
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        byte[] bytes;
        await using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms, ct);
            bytes = ms.ToArray();
        }

        var result = await service.ChangeAvatarAsync(userId, bytes, file.ContentType, ct);

        return result.ToActionResult(HttpContext);
    }
}
