using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class CurrencyRepository(OrbitaDbContext db) : ICurrencyRepository
{
    public async Task<List<Currency>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await db.Currencies
            .OrderBy(x => x.Kind)
            .ThenBy(x => x.Code)
            .ToListAsync(ct);

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<Currency?> GetAsync(string code, CancellationToken ct = default)
    {
        var normalized = code.Trim().ToUpperInvariant();
        var entity = await db.Currencies.FirstOrDefaultAsync(x => x.Code == normalized, ct);
        return entity?.ToDomain();
    }

    public async Task<Currency> CreateAsync(Currency currency, CancellationToken ct = default)
    {
        var entity = currency.ToEntity();
        await db.Currencies.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity.ToDomain();
    }

    public async Task UpsertManyAsync(IEnumerable<Currency> currencies, CancellationToken ct = default)
    {
        var list = currencies.ToList();
        if (list.Count == 0) return;

        var codes = list.Select(x => x.Code).ToList();
        var existing = await db.Currencies
            .Where(x => codes.Contains(x.Code))
            .ToDictionaryAsync(x => x.Code, ct);

        foreach (var currency in list)
        {
            if (existing.TryGetValue(currency.Code, out var entity))
            {
                entity.Name = currency.Name;
                entity.NumCode = currency.NumCode;
                entity.Kind = currency.Kind;
                entity.RateToRub = currency.RateToRub;
                entity.Nominal = currency.Nominal;
                entity.RateFetchedAt = currency.RateFetchedAt;
            }
            else
            {
                await db.Currencies.AddAsync(currency.ToEntity(), ct);
            }
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task<DateTime?> GetLatestFetchedAtAsync(Orbita.Domain.Enums.CurrencyKind? kind = null, CancellationToken ct = default)
    {
        var query = db.Currencies.Where(x => x.RateFetchedAt != null);
        if (kind.HasValue)
            query = query.Where(x => x.Kind == kind.Value);

        return await query.MaxAsync(x => (DateTime?)x.RateFetchedAt, ct);
    }
}
