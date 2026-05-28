namespace Orbita.Infrastructure.Entities;

public class AccountEntity
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public Guid TeamId { get; set; }
    public string Name { get; set; } = default!;
    public string CurrencyCode { get; set; } = default!;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public CurrencyEntity? Currency { get; set; }
}
