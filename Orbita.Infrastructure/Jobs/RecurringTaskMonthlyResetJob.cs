using Microsoft.Extensions.Logging;
using Orbita.Application.Abstractions.Jobs;
using Orbita.Application.Abstractions.Repositories;

namespace Orbita.Infrastructure.Jobs;

/// <summary>
/// На старте каждого календарного месяца (UTC) сбрасывает IsCompleted у всех RecurringTask.
/// Идемпотентно: повторные запуски в течение того же месяца ничего не делают благодаря
/// проверке LastResetAt &gt;= начала текущего месяца. Если job не выполнялся 1-го числа
/// (например, из-за простоя сервиса), он отработает при первом же запуске того же месяца.
/// </summary>
public class RecurringTaskMonthlyResetJob(
    IRecurringTaskRepository recurringTaskRepository,
    ILogger<RecurringTaskMonthlyResetJob> logger) : IDailyJob
{
    public string Name => "RecurringTaskMonthlyReset";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var cycleStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        try
        {
            var affected = await recurringTaskRepository.ResetCompletedForCycleAsync(cycleStart, now, ct);

            if (affected > 0)
            {
                logger.LogInformation(
                    "RecurringTask reset: cleared IsCompleted on {Count} task(s) for cycle starting {CycleStart:O}.",
                    affected, cycleStart);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "RecurringTaskMonthlyResetJob failed");
        }
    }
}
