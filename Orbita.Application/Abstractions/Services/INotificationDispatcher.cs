using Orbita.Contracts.ApiDto.Notifications;
using Orbita.Domain.Enums;

namespace Orbita.Application.Abstractions.Services;

public interface INotificationDispatcher
{
    Task<AppNotificationResponse> SendAsync(Guid userId, NotificationType type, string title, string message, bool pushOverHub = true, CancellationToken ct = default);

    /// <summary>
    /// Отправляет уведомление всем участникам команды. Для каждого члена создаётся отдельная запись
    /// (собственный Id, собственный флаг Read). Опционально можно исключить одного пользователя —
    /// например, инициатора действия, чтобы он не получал уведомление о собственных изменениях.
    /// </summary>
    Task<IReadOnlyList<AppNotificationResponse>> SendToTeamAsync(
        Guid teamId,
        NotificationType type,
        string title,
        string message,
        Guid? excludeUserId = null,
        bool pushOverHub = true,
        CancellationToken ct = default);
}
