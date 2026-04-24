namespace Orbita.Infrastructure.Entities;

public class RecurringPaymentEntity
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public Guid TeamId { get; set; }
    public string Title { get; set; } = default!;
    public long Amount { get; set; }
    public int DayOfMonth { get; set; }
    public Guid? CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
