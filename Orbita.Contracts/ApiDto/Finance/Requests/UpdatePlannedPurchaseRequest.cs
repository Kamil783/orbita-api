namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class UpdatePlannedPurchaseRequest
{
    public string? Title { get; set; }
    /// <summary>ISO date "YYYY-MM-DD".</summary>
    public string? Date { get; set; }
    public long? Amount { get; set; }

    public Guid? AssigneeId { get; set; }
    /// <summary>Если true — назначение сбрасывается, AssigneeId игнорируется.</summary>
    public bool ClearAssignee { get; set; }

    public Guid? CategoryId { get; set; }
    /// <summary>Если true — категория сбрасывается, CategoryId игнорируется.</summary>
    public bool ClearCategory { get; set; }

    public string? Note { get; set; }
    /// <summary>Если true — заметка сбрасывается, Note игнорируется.</summary>
    public bool ClearNote { get; set; }

    /// <summary>"planned" | "bought" | "cancelled".</summary>
    public string? Status { get; set; }
}
