namespace Orbita.Infrastructure.Entities;

public class FinanceBalanceEntity
{
    public Guid UserId { get; set; }
    public long Balance { get; set; }
    public DateTime UpdatedAt { get; set; }
}
