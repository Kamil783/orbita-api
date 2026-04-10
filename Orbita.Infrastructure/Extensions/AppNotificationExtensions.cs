using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class AppNotificationExtensions
{
    public static AppNotificationEntity ToEntity(this AppNotification n)
    {
        return new AppNotificationEntity
        {
            Id = n.Id.Id,
            UserId = n.UserId.Id,
            Type = n.Type,
            Title = n.Title,
            Message = n.Message,
            CreatedAt = n.CreatedAt,
            Read = n.Read
        };
    }

    public static AppNotification ToDomain(this AppNotificationEntity e)
    {
        return AppNotification.Restore(
            id: new AppNotificationId(e.Id),
            userId: new UserId(e.UserId),
            type: e.Type,
            title: e.Title,
            message: e.Message,
            createdAt: e.CreatedAt,
            read: e.Read
        );
    }
}
