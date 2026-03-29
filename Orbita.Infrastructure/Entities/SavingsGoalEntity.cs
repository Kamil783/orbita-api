namespace Orbita.Infrastructure.Entities;

public class SavingsGoalEntity
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public Guid TeamId { get; set; }
    public string Name { get; set; } = default!;
    public long Target { get; set; }
    public long Current { get; set; }
}
