using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Services;

public interface ITeamService
{
    Task<Result<Team>> CreateAsync(Guid userId, string name, CancellationToken ct = default);
    Task<Result<List<Team>>> GetAsync(CancellationToken ct = default);
    Task<Result> AddMemberAsync(Guid teamId, Guid targetUserId, CancellationToken ct = default);
}
