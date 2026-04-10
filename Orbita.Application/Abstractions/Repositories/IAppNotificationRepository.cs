using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;

namespace Orbita.Application.Abstractions.Repositories;

public interface IAppNotificationRepository
{
    Task<IReadOnlyList<AppNotification>> GetForUserAsync(Guid userId, int limit, CancellationToken ct = default);
    Task<AppNotification?> GetByIdAsync(AppNotificationId id, CancellationToken ct = default);
    Task AddAsync(AppNotification notification, CancellationToken ct = default);
    Task<bool> MarkAsReadAsync(AppNotificationId id, Guid userId, CancellationToken ct = default);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken ct = default);
}
