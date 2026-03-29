namespace Orbita.Application.Abstractions.Services;

public interface ITeamProvider
{
    Task<Guid> GetTeamIdAsync(Guid userId, CancellationToken ct = default);
}
