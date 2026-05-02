using Microsoft.Extensions.Logging;
using Orbita.Application.Abstractions;
using Orbita.Application.Abstractions.Jobs;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Domain.Enums;

namespace Orbita.Infrastructure.Jobs;

/// <summary>
/// Раз в час сканирует активные задачи с просроченным DueDate, у которых ещё не было
/// отправлено уведомление о просрочке (OverdueNotifiedAt == null), и шлёт каждому
/// исполнителю персональный Alert. После успешной рассылки помечает задачу как
/// уведомлённую — повторного спама не будет, пока юзер не сдвинет DueDate
/// (тогда BacklogTask.SetDueDate сбрасывает флаг).
/// </summary>
public class OverdueTaskNotificationJob(
    IBacklogTaskRepository backlogTaskRepository,
    INotificationDispatcher notificationDispatcher,
    IUnitOfWork unitOfWork,
    ILogger<OverdueTaskNotificationJob> logger) : IDailyJob
{
    public string Name => "OverdueTaskNotification";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var overdueTasks = await backlogTaskRepository.GetOverdueUnnotifiedAsync(now, ct);

        foreach (var task in overdueTasks)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                var assignees = task.Assignees.Select(a => a.Id).Distinct().ToList();

                var dueText = task.DueDate!.Value.ToString("dd.MM.yyyy");
                var message = $"Задача «{task.Title}» просрочена (срок был {dueText}).";

                foreach (var userId in assignees)
                {
                    await notificationDispatcher.SendAsync(
                        userId,
                        NotificationType.Alert,
                        title: "Задача просрочена",
                        message: message,
                        pushOverHub: true,
                        ct);
                }

                // Помечаем как уведомлённую даже если assignees пуст —
                // иначе job будет каждый час впустую обходить осиротевшие задачи.
                task.MarkOverdueNotified(now);
                await backlogTaskRepository.UpdateAsync(task, ct);

                await unitOfWork.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(ct);
                logger.LogError(
                    ex,
                    "Overdue notification failed for task {TaskId}; continuing.",
                    task.Id.Id);
            }
        }
    }
}
