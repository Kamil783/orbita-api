using Microsoft.AspNetCore.SignalR;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Contracts.ApiDto.Notifications;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Infrastructure.Notifications;

public class NotificationDispatcher(
    IAppNotificationRepository repository,
    IHubContext<NotificationsHub> hubContext) : INotificationDispatcher
{
    public async Task<AppNotificationResponse> SendAsync(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        bool pushOverHub = true,
        CancellationToken ct = default)
    {
        var notification = AppNotification.Create(new UserId(userId), type, title, message);
        await repository.AddAsync(notification, ct);

        var dto = ToResponse(notification);

        if (pushOverHub)
        {
            await hubContext.Clients
                .Group(userId.ToString())
                .SendAsync("ReceiveNotification", dto, ct);
        }

        return dto;
    }

    public static AppNotificationResponse ToResponse(AppNotification n) => new()
    {
        Id = n.Id.Id,
        Type = MapType(n.Type),
        Title = n.Title,
        Message = n.Message,
        Read = n.Read,
        CreatedAt = DateTime.SpecifyKind(n.CreatedAt, DateTimeKind.Utc)
    };

    public static string MapType(NotificationType type) => type switch
    {
        NotificationType.Task => "task",
        NotificationType.Meeting => "meeting",
        NotificationType.Finance => "finance",
        NotificationType.Alert => "alert",
        _ => "alert"
    };
}
