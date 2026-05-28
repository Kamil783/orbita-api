using Orbita.Domain.Enums;

namespace Orbita.Infrastructure.Entities;

public class CurrencyEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int? NumCode { get; set; }
    public CurrencyKind Kind { get; set; }
    public decimal? RateToRub { get; set; }
    public int Nominal { get; set; }
    public DateTime? RateFetchedAt { get; set; }

    public ICollection<AccountEntity> Accounts { get; set; } = [];
}
