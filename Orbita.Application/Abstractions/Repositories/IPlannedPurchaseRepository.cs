using Orbita.Domain.Entities;
using Orbita.Domain.Enums;

namespace Orbita.Application.Abstractions.Repositories;

public interface IPlannedPurchaseRepository
{
    Task<List<PlannedPurchase>> GetByTeamAsync(
        Guid teamId,
        DateOnly? from,
        DateOnly? to,
        PlannedPurchaseStatus? status,
        PlannedPurchaseDirection? direction,
        PlannedPurchaseAssigneeKind? assigneeKind,
        Guid? assigneeUserId,
        Guid? categoryId,
        CancellationToken ct = default);

    Task<PlannedPurchase?> GetAsync(Guid id, CancellationToken ct = default);
    Task<PlannedPurchase> CreateAsync(PlannedPurchase purchase, CancellationToken ct = default);
    Task<PlannedPurchase> UpdateAsync(PlannedPurchase purchase, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
