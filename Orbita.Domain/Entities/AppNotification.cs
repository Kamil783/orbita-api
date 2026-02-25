using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class AppNotification
{
    public AppNotificationId Id { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool Read {  get; private set; }
}
