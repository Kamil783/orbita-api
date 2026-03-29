namespace Orbita.Infrastructure.Entities;

public class FinanceTransactionEntity
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public Guid TeamId { get; set; }
    public Guid? CategoryId { get; set; }
    public FinanceCategoryEntity? Category { get; set; }
    public string Title { get; set; } = default!;
    public long Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsFromBalance { get; set; }
}
