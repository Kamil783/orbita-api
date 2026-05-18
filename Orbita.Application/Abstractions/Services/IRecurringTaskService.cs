using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Services;

public interface IRecurringTaskService
{
    Task<Result<List<RecurringTask>>> GetAsync(Guid userId, CancellationToken ct = default);

    Task<Result<RecurringTask>> CreateAsync(
        Guid userId,
        string title,
        string? description,
        int dayOfMonth,
        CancellationToken ct = default);

    Task<Result<RecurringTask>> UpdateAsync(
        Guid userId,
        Guid taskId,
        string? title,
        string? description,
        bool clearDescription,
        int? dayOfMonth,
        bool? isCompleted,
        CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid userId, Guid taskId, CancellationToken ct = default);
}
