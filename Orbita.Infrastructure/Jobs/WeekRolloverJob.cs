using Microsoft.Extensions.Logging;
using Orbita.Application.Abstractions.Jobs;
using Orbita.Application.Abstractions.Repositories;

namespace Orbita.Infrastructure.Jobs;

public class WeekRolloverJob(
    IWeekRepository weekRepository,
    ILogger<MonthRolloverJob> logger) : IDailyJob
{
    public string Name => "WeekRollover";

    public Task ExecuteAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var currentWeekStart = GetStartOfWeek(now, DayOfWeek.Monday);
        throw new NotImplementedException();

    }

    private static DateTime GetStartOfWeek(DateTime date, DayOfWeek startOfWeek)
    {
        var diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
        var start = date.Date.AddDays(-diff);

        return DateTime.SpecifyKind(start, DateTimeKind.Utc);
    }
}
