using Orbita.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Orbita.Application.Abstractions.Services;
using Orbita.Contracts.ApiDto.User.Requests;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Authenticate([FromBody] LoginDto request, CancellationToken ct)
    {
        var result = await service.AuthenticateAsync(request.ToCommand(), ct);

        return result.ToActionResult(HttpContext);
    }

    [HttpPost("registration")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request, CancellationToken ct)
    {
        var result = await service.RegisterAsync(request.ToCommand(), ct);

        return result.ToActionResult(HttpContext);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto request, CancellationToken ct)
    {
        var result = await service.RefreshAsync(request.RefreshToken, ct);

        return result.ToActionResult(HttpContext);
    }
}
