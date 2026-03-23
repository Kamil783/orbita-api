using Orbita.Domain.Enums;

namespace Orbita.Infrastructure.Entities;

public class AppNotificationEntity
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public bool Read { get; set; }
}
