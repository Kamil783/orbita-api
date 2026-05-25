namespace Orbita.Contracts.ApiDto.Finance.Requests;

/// <summary>
/// PATCH-семантика:
///  * Не-nullable поля (Title / Date / Direction / Amount / Status): null = не трогать.
///  * Nullable поля (ActualAmount / AssigneeKind / AssigneeUserId / CategoryId / Note):
///    значение из тела всегда записывается; null = снять. Клиент должен присылать
///    желаемое финальное состояние этих полей.
/// </summary>
public sealed class UpdatePlannedPurchaseRequest
{
    public string? Title { get; set; }
    /// <summary>ISO date "YYYY-MM-DD".</summary>
    public string? Date { get; set; }

    /// <summary>"expense" | "income". null = не трогать.</summary>
    public string? Direction { get; set; }

    public long? Amount { get; set; }
    /// <summary>Фактическая сумма; null = снять; &gt; 0 если задана.</summary>
    public long? ActualAmount { get; set; }

    /// <summary>"user" | "team" | null. null — снять назначение.</summary>
    public string? AssigneeKind { get; set; }
    /// <summary>Обязателен при AssigneeKind = "user". Для kind = "team" / null — игнорируется.</summary>
    public Guid? AssigneeUserId { get; set; }

    public Guid? CategoryId { get; set; }
    public string? Note { get; set; }

    /// <summary>"planned" | "bought" | "cancelled". null = не трогать.</summary>
    public string? Status { get; set; }
}
