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
    ITeamRepository teamRepository,
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

    public async Task<IReadOnlyList<AppNotificationResponse>> SendToTeamAsync(
        Guid teamId,
        NotificationType type,
        string title,
        string message,
        Guid? excludeUserId = null,
        bool pushOverHub = true,
        CancellationToken ct = default)
    {
        var team = await teamRepository.GetAsync(teamId, ct);
        if (team is null)
            return Array.Empty<AppNotificationResponse>();

        var recipients = team.TeamMembers
            .Select(m => m.UserId.Id)
            .Where(id => excludeUserId is null || id != excludeUserId.Value)
            .Distinct()
            .ToList();

        if (recipients.Count == 0)
            return Array.Empty<AppNotificationResponse>();

        var results = new List<AppNotificationResponse>(recipients.Count);

        foreach (var userId in recipients)
        {
            var notification = AppNotification.Create(new UserId(userId), type, title, message);
            await repository.AddAsync(notification, ct);
            results.Add(ToResponse(notification));
        }

        if (pushOverHub)
        {
            for (var i = 0; i < recipients.Count; i++)
            {
                await hubContext.Clients
                    .Group(recipients[i].ToString())
                    .SendAsync("ReceiveNotification", results[i], ct);
            }
        }

        return results;
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
