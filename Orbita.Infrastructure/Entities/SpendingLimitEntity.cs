namespace Orbita.Infrastructure.Entities;

public class SpendingLimitEntity
{
    public Guid UserId { get; set; }
    public long MonthlyLimit { get; set; }
    public long WeeklyLimit { get; set; }
}
