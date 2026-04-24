namespace Orbita.Contracts.ApiDto.Finance.Requests;

public sealed class CreateRecurringPaymentRequest
{
    public required string Title { get; set; }
    public required long Amount { get; set; }
    public required int DayOfMonth { get; set; }
    public Guid? CategoryId { get; set; }
}
