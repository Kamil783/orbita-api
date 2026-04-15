using Microsoft.Extensions.Logging;
using Orbita.Application.Abstractions;
using Orbita.Application.Abstractions.Jobs;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;

namespace Orbita.Infrastructure.Jobs;

public class WeekRolloverJob(
    IBacklogTaskRepository backlogTaskRepository,
    ITeamRepository teamRepository,
    IWeekRepository weekRepository,
    IUnitOfWork unitOfWork,
    ILogger<MonthRolloverJob> logger) : IDailyJob
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
            var week = await weekRepository.GetCurrentAsync(team.Id.Id, ct);
            var shouldCreateNewWeek = false;

            if (week is null)
            {
                shouldCreateNewWeek = true;
            }
            else if (week.StartDate < currentWeekStart)
            {
                week.Archive();
                await weekRepository.UpdateAsync(week, ct);
                shouldCreateNewWeek = true;
            }

            if (shouldCreateNewWeek)
            {
                var newWeek = Week.Create(
                    teamId: team.Id,
                    creatorId: null,
                    startDate: currentWeekStart,
                    endDate: currentWeekEnd
                );            

                var backlogTasks = await backlogTaskRepository.GetActiveByTeamAsync(team.Id.Id, ct);

                foreach (var backlogTask in backlogTasks)
                {
                    newWeek.AddTask(backlogTask.Id);
                }

                await weekRepository.CreateAsync(newWeek, ct);
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
