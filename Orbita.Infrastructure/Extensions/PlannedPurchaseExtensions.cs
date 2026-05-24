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
            Amount = p.Amount,
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
            amount: e.Amount,
            assigneeKind: e.AssigneeKind,
            assigneeUserId: e.AssigneeUserId.HasValue ? new UserId(e.AssigneeUserId.Value) : null,
            categoryId: e.CategoryId.HasValue ? new FinanceCategoryId(e.CategoryId.Value) : null,
            note: e.Note,
            status: e.Status,
            createdAt: e.CreatedAt,
            updatedAt: e.UpdatedAt);
    }
}
