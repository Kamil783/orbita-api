using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Orbita.Api.Controllers;

[ApiController]
[Authorize]
public abstract class AuthorizedControllerBase : ControllerBase
{
    protected bool TryGetUserId(out Guid userId)
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return Guid.TryParse(userIdValue, out userId);
    }
}