using Orbita.Domain.Enums;

namespace Orbita.Infrastructure.Entities;

public class PlannedPurchaseEntity
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public Guid TeamId { get; set; }
    public string Title { get; set; } = default!;
    public DateOnly Date { get; set; }
    public PlannedPurchaseDirection Direction { get; set; }
    public long Amount { get; set; }
    public long? ActualAmount { get; set; }
    public PlannedPurchaseAssigneeKind? AssigneeKind { get; set; }
    public Guid? AssigneeUserId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Note { get; set; }
    public PlannedPurchaseStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
