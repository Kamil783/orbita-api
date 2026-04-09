using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class AppNotification
{
    public AppNotificationId Id { get; private set; }
    public UserId UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool Read { get; private set; }

    private AppNotification() { }

    public static AppNotification Create(UserId userId, NotificationType type, string title, string message)
    {
        return new AppNotification
        {
            Id = new AppNotificationId(Guid.NewGuid()),
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            Read = false
        };
    }

    public static AppNotification Restore(AppNotificationId id, UserId userId, NotificationType type, string title, string message, DateTime createdAt, bool read)
    {
        return new AppNotification
        {
            Id = id,
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            CreatedAt = createdAt,
            Read = read
        };
    }

    public void MarkAsRead() => Read = true;
}
