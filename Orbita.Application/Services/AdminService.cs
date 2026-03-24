using Orbita.Application.Abstractions.Gateways;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Dto;
using Orbita.Application.Models.Results;

namespace Orbita.Application.Services;

public class AdminService(
    IIdentityUserGateway userGateway,
    IBacklogTaskRepository backlogTaskRepository,
    IUserProfileRepository userProfileRepository) : IAdminService
{
    public async Task<Result<List<AdminUserData>>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await userGateway.GetAllUsersAsync(ct);
        return Result<List<AdminUserData>>.Ok(users.ToList());
    }

    public async Task<Result<List<AdminBacklogTaskData>>> GetAllBacklogTasksAsync(CancellationToken ct = default)
    {
        var tasks = await backlogTaskRepository.GetAllAsync(ct);
        var profiles = await userProfileRepository.GetTeamUserProfilesAsync(Guid.Empty, ct);
        var profileMap = profiles.ToDictionary(p => p.UserId.Id, p => p.Name);

        var result = tasks.Select(t => new AdminBacklogTaskData(
            t.Id.Id,
            t.Title,
            t.Priority,
            t.IsCompleted,
            t.InWeek,
            t.CreatorId.Id,
            profileMap.GetValueOrDefault(t.CreatorId.Id),
            t.Assignees.Select(a => a.Id).ToList(),
            t.DueDate,
            t.CreatedAt)).ToList();

        return Result<List<AdminBacklogTaskData>>.Ok(result);
    }
}
