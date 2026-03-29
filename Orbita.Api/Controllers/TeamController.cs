using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;

namespace Orbita.Api.Controllers
{
    [Route("api/[controller]")]
    public class TeamController(IUserService service) : AuthorizedControllerBase
    {
        [HttpGet("members")]
        public async Task<IActionResult> GetMembers(CancellationToken ct)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var result = await service.GetTeamDataAsync(userId, ct);

            return result.ToActionResult(HttpContext);
        }
    }
}
