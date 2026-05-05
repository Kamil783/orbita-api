using Microsoft.AspNetCore.Mvc;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Contracts.ApiDto.Notifications;
using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Notifications;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
public class NotificationsController(
    IAppNotificationRepository repository,
    INotificationDispatcher dispatcher) : AuthorizedControllerBase
{
    private const int DefaultLimit = 100;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var items = await repository.GetForUserAsync(userId, DefaultLimit, ct);
        var response = items.Select(NotificationDispatcher.ToResponse).ToList();
        return Ok(response);
    }

    [HttpPost("test")]
    public async Task<IActionResult> Test(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var dto = await dispatcher.SendAsync(
            userId,
            NotificationType.Alert,
            "Тестовое уведомление",
            "Это проверочное уведомление, отправленное вручную из профиля.",
            pushOverHub: false,
            ct);

        return Ok(dto);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var ok = await repository.MarkAsReadAsync(new AppNotificationId(id), userId, ct);
        if (!ok)
            return NotFound();

        return NoContent();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        await repository.MarkAllAsReadAsync(userId, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var ok = await repository.DeleteAsync(new AppNotificationId(id), userId, ct);
        if (!ok)
            return NotFound();

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAll(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        await repository.DeleteAllForUserAsync(userId, ct);
        return NoContent();
    }
}
