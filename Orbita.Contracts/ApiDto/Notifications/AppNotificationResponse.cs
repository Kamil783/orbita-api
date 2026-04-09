namespace Orbita.Contracts.ApiDto.Notifications;

public class AppNotificationResponse
{
    public Guid Id { get; set; }
    public string Type { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public bool Read { get; set; }
    public DateTime CreatedAt { get; set; }
}
