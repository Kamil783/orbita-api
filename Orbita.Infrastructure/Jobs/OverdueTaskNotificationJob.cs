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

    private static string DaysWord(int days)
    {
        var mod100 = days % 100;
        if (mod100 is >= 11 and <= 14) return "дней";
        var mod10 = days % 10;
        return mod10 switch
        {
            1 => "день",
            2 or 3 or 4 => "дня",
            _ => "дней"
        };
    }

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

                var due = task.DueDate!.Value;
                var dueText = due.ToString("dd.MM.yyyy");
                var daysOverdue = Math.Max(1, (int)Math.Floor((now.Date - due.Date).TotalDays));
                var daysWord = DaysWord(daysOverdue);
                var message = $"Задача «{task.Title}» просрочена. Срок был {dueText}, прошло {daysOverdue} {daysWord}.";

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
