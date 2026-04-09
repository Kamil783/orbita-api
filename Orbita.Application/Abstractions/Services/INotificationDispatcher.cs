using Orbita.Contracts.ApiDto.Notifications;
using Orbita.Domain.Enums;

namespace Orbita.Application.Abstractions.Services;

public interface INotificationDispatcher
{
    Task<AppNotificationResponse> SendAsync(Guid userId, NotificationType type, string title, string message, bool pushOverHub = true, CancellationToken ct = default);
}
