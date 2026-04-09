using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class AppNotificationRepository(OrbitaDbContext db) : IAppNotificationRepository
{
    public async Task<IReadOnlyList<AppNotification>> GetForUserAsync(Guid userId, int limit, CancellationToken ct = default)
    {
        var entities = await db.AppNotifications
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<AppNotification?> GetByIdAsync(AppNotificationId id, CancellationToken ct = default)
    {
        var e = await db.AppNotifications
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id.Id, ct);

        return e?.ToDomain();
    }

    public async Task AddAsync(AppNotification notification, CancellationToken ct = default)
    {
        db.AppNotifications.Add(notification.ToEntity());
        await db.SaveChangesAsync(ct);
    }

    public async Task<bool> MarkAsReadAsync(AppNotificationId id, Guid userId, CancellationToken ct = default)
    {
        var entity = await db.AppNotifications
            .FirstOrDefaultAsync(x => x.Id == id.Id && x.UserId == userId, ct);

        if (entity is null)
            return false;

        if (!entity.Read)
        {
            entity.Read = true;
            await db.SaveChangesAsync(ct);
        }

        return true;
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken ct = default)
    {
        await db.AppNotifications
            .Where(x => x.UserId == userId && !x.Read)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Read, true), ct);
    }
}
