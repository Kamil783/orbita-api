using Microsoft.Extensions.Logging;
using Orbita.Application.Abstractions;
using Orbita.Application.Abstractions.Jobs;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;

namespace Orbita.Infrastructure.Jobs;

public class WeekRolloverJob(
    ITeamRepository teamRepository,
    IWeekRepository weekRepository,
    IWeekService weekService,
    IUnitOfWork unitOfWork,
    ILogger<WeekRolloverJob> logger) : IDailyJob
{
    public string Name => "WeekRollover";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var teams = await teamRepository.GetAllAsync(ct);

        var now = DateTime.UtcNow;
        var currentWeekStart = GetStartOfWeek(now, DayOfWeek.Monday);
        var currentWeekEnd = currentWeekStart.AddDays(6);

        foreach (var team in teams)
        {
            try
            {
                var week = await weekRepository.GetCurrentAsync(team.Id.Id, ct);

                var shouldCreateNewWeek =
                    week is null ||
                    week.StartDate.Date < currentWeekStart.Date;

                if (!shouldCreateNewWeek)
                    continue;

                var result = await unitOfWork.ExecuteAsync(
                    token => weekService.CreateNewWeekForTeamAsync(
                        team.Id.Id,
                        creatorId: null,
                        startDate: currentWeekStart,
                        endDate: currentWeekEnd,
                        token),
                    ct);

                if (!result.IsSuccess)
                {
                    logger.LogWarning(
                        "Failed to rollover week for team {TeamId}: {Error} ({Type})",
                        team.Id.Id,
                        result.Error?.Message,
                        result.Error?.Type);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Week rollover threw for team {TeamId}; continuing with remaining teams.",
                    team.Id.Id);
            }
        }
    }

    private static DateTime GetStartOfWeek(DateTime date, DayOfWeek startOfWeek)
    {
        var diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
        var start = date.Date.AddDays(-diff);

        return DateTime.SpecifyKind(start, DateTimeKind.Utc);
    }
}
