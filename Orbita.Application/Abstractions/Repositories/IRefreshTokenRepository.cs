using Orbita.Application.Models.Dto;

namespace Orbita.Application.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(string token, Guid userId, DateTime expiresAt, CancellationToken ct = default);
    Task<RefreshTokenData?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<bool> TryRevokeAsync(string token, CancellationToken ct = default);
}
