using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;

namespace Orbita.Application.Services;

public class WeekService(
    IWeekRepository weekRepository,
    IBacklogTaskRepository backlogTaskRepository,
    ITeamProvider teamProvider) : IWeekService
{
    public async Task<Result<Week>> CreateNewWeekAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var currentWeek = await weekRepository.GetCurrentAsync(teamId, ct);
        if (currentWeek is not null)
        {
            if (currentWeek.StartDate.Date == startDate.Date && currentWeek.EndDate.Date == endDate.Date)
                return Result<Week>.Conflict("Week already exist.");
            currentWeek.Archive();
            await weekRepository.UpdateAsync(currentWeek, ct);
        }

        var newWeek = Week.Create(
            creatorId: new UserId(userId),
            teamId: new TeamId(teamId),
            startDate: startDate,
            endDate: endDate);

        var inWeekTasks = await backlogTaskRepository.GetByTeamAsync(teamId, ct);
        foreach (var task in inWeekTasks.Where(t => t.InWeek && t.IsCompleted))
        {
            newWeek.AddTask(task.Id);
        }
        foreach (var task in inWeekTasks.Where(t => t.InWeek && !t.IsCompleted))
        {
            newWeek.AddTask(task.Id);
        }

        var created = await weekRepository.CreateAsync(newWeek, ct);

        return Result<Week>.Ok(created);
    }

    public async Task<Result<List<(Week Week, List<BacklogTask> Tasks)>>> GetArchivesAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var archivedWeeks = await weekRepository.GetArchivedAsync(teamId, ct);
        var result = new List<(Week Week, List<BacklogTask> Tasks)>();

        foreach (var week in archivedWeeks)
        {
            var tasks = new List<BacklogTask>();
            foreach (var taskId in week.TaskIds)
            {
                var task = await backlogTaskRepository.GetAsync(taskId.Id, ct);
                if (task is not null)
                    tasks.Add(task);
            }
            result.Add((week, tasks));
        }

        return Result<List<(Week Week, List<BacklogTask> Tasks)>>.Ok(result);
    }

    public async Task<List<Week>> GetWeeksByBacklogTaskAsync(Guid backlogTaskId, CancellationToken ct = default)
    {
        return await weekRepository.GetByBacklogTaskAsync(backlogTaskId, ct);
    }

    public async Task<Week?> GetCurrentWeekAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        return await weekRepository.GetCurrentAsync(teamId, ct);
    }
}
