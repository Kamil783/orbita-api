namespace Orbita.Infrastructure.Entities;

public class FinanceCategoryEntity
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public Guid TeamId { get; set; }
    public string Name { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public string Bg { get; set; } = default!;
    public string Color { get; set; } = default!;
    public long? WeeklyLimit { get; set; }
    public long? MonthlyLimit { get; set; }
}
