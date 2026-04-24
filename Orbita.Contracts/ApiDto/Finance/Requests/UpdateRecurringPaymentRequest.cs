namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class UpdateRecurringPaymentRequest
{
    public string? Title { get; set; }
    public long? Amount { get; set; }
    public int? DayOfMonth { get; set; }
    /// <summary>Новая категория. Если null и ClearCategory = false — категория не меняется.</summary>
    public Guid? CategoryId { get; set; }
    /// <summary>Если true — категория сбрасывается в null, CategoryId игнорируется.</summary>
    public bool ClearCategory { get; set; }
}
