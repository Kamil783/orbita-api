namespace Orbita.Infrastructure.Entities;

public class AccountTransactionEntity
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid CreatorId { get; set; }
    public Guid TeamId { get; set; }
    public Guid? CategoryId { get; set; }
    public string Title { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }

    public AccountEntity? Account { get; set; }
}
