using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class PlannedPurchaseExtensions
{
    public static PlannedPurchaseEntity ToEntity(this PlannedPurchase p)
    {
        return new PlannedPurchaseEntity
        {
            Id = p.Id.Id,
            OwnerId = p.OwnerId.Id,
            TeamId = p.TeamId.Id,
            Title = p.Title,
            Date = p.Date,
            Direction = p.Direction,
            Amount = p.Amount,
            ActualAmount = p.ActualAmount,
            AssigneeKind = p.AssigneeKind,
            AssigneeUserId = p.AssigneeUserId?.Id,
            CategoryId = p.CategoryId?.Id,
            Note = p.Note,
            Status = p.Status,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }

    public static PlannedPurchase ToDomain(this PlannedPurchaseEntity e)
    {
        return PlannedPurchase.Restore(
            id: new PlannedPurchaseId(e.Id),
            ownerId: new UserId(e.OwnerId),
            teamId: new TeamId(e.TeamId),
            title: e.Title,
            date: e.Date,
            direction: e.Direction,
            amount: e.Amount,
            actualAmount: e.ActualAmount,
            assigneeKind: e.AssigneeKind,
            assigneeUserId: e.AssigneeUserId.HasValue ? new UserId(e.AssigneeUserId.Value) : null,
            categoryId: e.CategoryId.HasValue ? new FinanceCategoryId(e.CategoryId.Value) : null,
            note: e.Note,
            status: e.Status,
            createdAt: e.CreatedAt,
            updatedAt: e.UpdatedAt);
    }
}
