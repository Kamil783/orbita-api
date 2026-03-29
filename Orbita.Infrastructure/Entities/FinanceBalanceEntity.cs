namespace Orbita.Infrastructure.Entities;

public class FinanceBalanceEntity
{
    public Guid TeamId { get; set; }
    public long Balance { get; set; }
    public long PreviousMonthBalance { get; set; }
    public DateTime? LastMonthClosedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
