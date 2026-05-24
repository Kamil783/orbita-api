namespace Orbita.Contracts.ApiDto.Finance.Responses;

public sealed class PlannedPurchaseResponse
{
    public string Id { get; init; } = default!;
    public string Title { get; init; } = default!;
    /// <summary>ISO date "YYYY-MM-DD".</summary>
    public string Date { get; init; } = default!;
    public long Amount { get; init; }

    /// <summary>"user" | "team" | null.</summary>
    public string? AssigneeKind { get; init; }
    /// <summary>Заполнен только при AssigneeKind == "user".</summary>
    public string? AssigneeUserId { get; init; }

    public string? CategoryId { get; init; }
    public string? Note { get; init; }
    /// <summary>"planned" | "bought" | "cancelled".</summary>
    public string Status { get; init; } = default!;
    public long CreatedAt { get; init; }
    public long UpdatedAt { get; init; }
}
