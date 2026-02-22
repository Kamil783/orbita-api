using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class RefreshTokenRepository(OrbitaDbContext db) : IRefreshTokenRepository
{
    public async Task AddAsync(string token, Guid userId, DateTime expiresAt, CancellationToken ct = default)
    {
        var entity = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = userId,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        db.RefreshTokens.Add(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task<RefreshTokenData?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        var entity = await db.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Token == token, ct);

        if (entity is null) return null;

        return new RefreshTokenData(entity.Token, entity.UserId, entity.ExpiresAt, entity.IsRevoked);
    }

    public async Task<bool> TryRevokeAsync(string token, CancellationToken ct = default)
    {
        var affected = await db.RefreshTokens
            .Where(t => t.Token == token && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsRevoked, true), ct);

        return affected > 0;
    }
}
