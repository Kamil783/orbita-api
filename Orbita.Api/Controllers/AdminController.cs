using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.Admin.Responses;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController(IAdminService adminService) : ControllerBase
{
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct)
    {
        var result = await adminService.GetAllUsersAsync(ct);

        return result
            .Map(users => users.Select(u => new AdminUserResponse
            {
                Id = u.UserId.ToString(),
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                Avatar = u.Avatar != null ? Convert.ToBase64String(u.Avatar) : null,
                CreatedAt = u.CreatedAt?.ToString("o")
            }).ToArray())
            .ToActionResult(HttpContext);
    }

    [HttpGet("backlog")]
    public async Task<IActionResult> GetAllBacklogTasks(CancellationToken ct)
    {
        var result = await adminService.GetAllBacklogTasksAsync(ct);

        return result
            .Map(tasks => tasks.Select(t => new AdminBacklogTaskResponse
            {
                Id = t.Id.ToString(),
                Title = t.Title,
                Priority = t.Priority.ToString(),
                IsCompleted = t.IsCompleted,
                InWeek = t.InWeek,
                CreatorId = t.CreatorId.ToString(),
                CreatorName = t.CreatorName,
                AssigneeIds = t.AssigneeIds.Select(a => a.ToString()).ToArray(),
                DueDate = t.DueDate?.ToString("o"),
                CreatedAt = t.CreatedAt.ToString("o")
            }).ToArray())
            .ToActionResult(HttpContext);
    }
}
