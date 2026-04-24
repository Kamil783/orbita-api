namespace Orbita.Contracts.ApiDto.Finance.Responses;

public sealed class RecurringPaymentResponse
{
    public string Id { get; init; } = default!;
    public string Title { get; init; } = default!;
    public long Amount { get; init; }
    public int DayOfMonth { get; init; }
    public string? CategoryId { get; init; }
    public long CreatedAt { get; init; }
    public long UpdatedAt { get; init; }
}
