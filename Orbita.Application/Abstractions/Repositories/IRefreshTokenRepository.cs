namespace Orbita.Application.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(string token, Guid userId, DateTime expiresAt, CancellationToken ct = default);
    Task<RefreshTokenData?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task RevokeAsync(string token, CancellationToken ct = default);
}

public record RefreshTokenData(string Token, Guid UserId, DateTime ExpiresAt, bool IsRevoked);
