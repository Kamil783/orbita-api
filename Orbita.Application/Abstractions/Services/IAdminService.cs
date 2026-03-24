using Orbita.Application.Models.Dto;
using Orbita.Application.Models.Results;

namespace Orbita.Application.Abstractions.Services;

public interface IAdminService
{
    Task<Result<List<AdminUserData>>> GetAllUsersAsync(CancellationToken ct = default);
    Task<Result<List<AdminBacklogTaskData>>> GetAllBacklogTasksAsync(CancellationToken ct = default);
}
