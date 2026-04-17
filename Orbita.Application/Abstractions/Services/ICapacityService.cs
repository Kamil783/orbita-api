using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Services;

public interface ICapacityService
{
    Task<Result<TeamCapacity>> GetAsync(Guid userId, CancellationToken ct = default);
    Task<Result<TeamCapacity>> UpdateAsync(Guid userId, int weekdayHours, int weekendHours, CancellationToken ct = default);
}
