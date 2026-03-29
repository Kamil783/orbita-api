using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Services;

public interface ITeamService
{
    Task<Result<Team>> CreateAsync(Guid userId, string name, CancellationToken ct = default);
    Task<Result> AddMemberAsync(Guid currentUserId, Guid targetUserId, CancellationToken ct = default);
}
