using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Services;

public interface IWeekService
{
    Task<Result> CreateNewWeekAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<Result<List<(Week Week, List<BacklogTask> Tasks)>>> GetArchivesAsync(Guid userId, CancellationToken ct = default);
    Task<List<Week>> GetWeeksByBacklogTaskAsync(Guid backlogTaskId, CancellationToken ct = default);
    Task<Week?> GetCurrentWeekAsync(Guid userId, CancellationToken ct = default);
}
