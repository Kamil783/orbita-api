using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orbita.Contracts.ApiDto.Tasks.Requests;
using System.Security.Claims;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
public class TasksController : AuthorizedControllerBase
{
    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeeklyTasks()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();


    }
}
