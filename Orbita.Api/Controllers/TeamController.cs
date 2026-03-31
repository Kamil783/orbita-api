using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.Team.Requests;

namespace Orbita.Api.Controllers
{
    [Route("api/[controller]")]
    public class TeamController(IUserService userService, ITeamService teamService) : AuthorizedControllerBase
    {
        [HttpGet("members")]
        public async Task<IActionResult> GetMembers(CancellationToken ct)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var result = await userService.GetTeamDataAsync(userId, ct);

            return result.ToActionResult(HttpContext);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTeamRequest request, CancellationToken ct)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var result = await teamService.CreateAsync(userId, request.Name, ct);

            return result
                .Map(t => new { id = t.Id.Id.ToString(), name = t.Name })
                .ToActionResult(HttpContext);
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var result = await teamService.GetAsync(ct);

            return result
                .Map(x => x.Select(t =>  new { id = t.Id.Id.ToString(), name = t.Name, members = t.TeamMembers.Select(tm => new { Id = tm.UserId, Name = tm.Name, Avatar = tm.AvatarData }) }))
                .ToActionResult(HttpContext);
        }

        [HttpPost("members")]
        public async Task<IActionResult> AddMember([FromBody] AddMemberRequest request, CancellationToken ct)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var result = await teamService.AddMemberAsync(request.TeamId, request.UserId, ct);

            return result.ToActionResult(HttpContext);
        }
    }
}
