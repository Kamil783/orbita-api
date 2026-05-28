using Orbita.Domain.Entities;
using Orbita.Domain.Enums;

namespace Orbita.Application.Abstractions.Repositories;

public interface ICurrencyRepository
{
    Task<List<Currency>> GetAllAsync(CancellationToken ct = default);
    Task<Currency?> GetAsync(string code, CancellationToken ct = default);
    Task<Currency> CreateAsync(Currency currency, CancellationToken ct = default);
    Task UpsertManyAsync(IEnumerable<Currency> currencies, CancellationToken ct = default);
    /// <summary>Самое последнее значение <see cref="Currency.RateFetchedAt"/>. Можно отфильтровать по типу валюты,
    /// чтобы независимо отслеживать фиат и крипту.</summary>
    Task<DateTime?> GetLatestFetchedAtAsync(CurrencyKind? kind = null, CancellationToken ct = default);
}
