using Orbita.Domain.Entities;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class CurrencyExtensions
{
    public static CurrencyEntity ToEntity(this Currency c) => new()
    {
        Code = c.Code,
        Name = c.Name,
        NumCode = c.NumCode,
        Kind = c.Kind,
        RateToRub = c.RateToRub,
        Nominal = c.Nominal,
        RateFetchedAt = c.RateFetchedAt
    };

    public static Currency ToDomain(this CurrencyEntity e) =>
        Currency.Restore(e.Code, e.Name, e.NumCode, e.Kind, e.RateToRub, e.Nominal, e.RateFetchedAt);
}
