using Microsoft.Extensions.Logging;
using Orbita.Application.Abstractions;
using Orbita.Application.Abstractions.Jobs;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Domain.Enums;

namespace Orbita.Infrastructure.Jobs;

/// <summary>
/// Раз в час сканирует RecurringTask, у которых истёк день месяца, задача не выполнена
/// и в текущие сутки ещё не отправлялось уведомление. Уведомляет создателя задачи
/// (исполнителей у этой модели нет). Поле LastOverdueNotifiedAt гарантирует, что
/// уведомление шлётся не чаще одного раза в сутки — DailyTaskRunnerService будет
/// заходить сюда каждый час, но дублей не пошлёт.
///
/// Сброс цикла (RecurringTaskMonthlyResetJob) обнуляет LastOverdueNotifiedAt вместе
/// с IsCompleted, чтобы новый месяц начинался «с чистого листа».
/// </summary>
public class RecurringTaskOverdueNotificationJob(
    IRecurringTaskRepository recurringTaskRepository,
    INotificationDispatcher notificationDispatcher,
    IUnitOfWork unitOfWork,
    ILogger<RecurringTaskOverdueNotificationJob> logger) : IDailyJob
{
    public string Name => "RecurringTaskOverdueNotification";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var tasks = await recurringTaskRepository.GetOverdueNotNotifiedTodayAsync(now, ct);

        foreach (var task in tasks)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                var dueDay = task.EffectiveDueDay(now);
                var message = task.Description is null
                    ? $"Ежемесячная задача «{task.Title}» просрочена (срок — {dueDay} число месяца, сегодня {now.Day})."
                    : $"Ежемесячная задача «{task.Title}» просрочена (срок — {dueDay} число месяца, сегодня {now.Day}). {task.Description}";

                await notificationDispatcher.SendAsync(
                    task.CreatorId.Id,
                    NotificationType.Alert,
                    title: "Ежемесячная задача просрочена",
                    message: message,
                    pushOverHub: true,
                    ct);

                task.MarkOverdueNotified(now);
                await recurringTaskRepository.UpdateAsync(task, ct);

                await unitOfWork.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(ct);
                logger.LogError(
                    ex,
                    "Recurring task overdue notification failed for task {TaskId}; continuing.",
                    task.Id.Id);
            }
        }
    }
}
