using Microsoft.Extensions.Logging;
using Orbita.Application.Abstractions.Jobs;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Infrastructure.ExternalServices;

namespace Orbita.Infrastructure.Jobs;

/// <summary>
/// Тянет курсы крипты с CoinGecko (simple/price API, RUB vs_currency) в те же слоты,
/// что и CBR-job: 07:00 / 14:00 / 20:00 UTC. Гейт по
/// <see cref="ICurrencyRepository.GetLatestFetchedAtAsync"/> по записям с
/// <see cref="CurrencyKind.Crypto"/> — фиатный фетч и крипто-фетч независимы.
/// </summary>
public class CoinGeckoRatesFetchJob(
    CoinGeckoRatesClient coinGeckoClient,
    ICurrencyRepository currencyRepository,
    ILogger<CoinGeckoRatesFetchJob> logger) : IDailyJob
{
    private static readonly int[] SlotsUtc = [7, 14, 20];

    public string Name => "CoinGeckoRatesFetch";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var slotStart = ResolveCurrentSlotStart(now);

        // Берём максимум FetchedAt только среди крипты — иначе CBR-job, отработав, заблокировал бы крипту.
        var lastFetchedAt = await currencyRepository.GetLatestFetchedAtAsync(CurrencyKind.Crypto, ct);
        if (lastFetchedAt is not null && lastFetchedAt.Value >= slotStart)
            return;

        IReadOnlyList<CryptoRate> rates;
        try
        {
            rates = await coinGeckoClient.FetchAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CoinGecko rates fetch failed");
            return;
        }

        if (rates.Count == 0)
        {
            logger.LogWarning("CoinGecko returned no rates; skipping upsert.");
            return;
        }

        var fetchedAt = DateTime.UtcNow;
        var currencies = rates.Select(r =>
        {
            var c = Currency.Create(r.Ticker, r.Name, numCode: null, CurrencyKind.Crypto);
            c.UpdateRate(r.RatePerUnitRub, nominal: 1, fetchedAt);
            return c;
        }).ToList();

        await currencyRepository.UpsertManyAsync(currencies, ct);

        logger.LogInformation(
            "CoinGecko rates updated: {Count} coins for slot {Slot:O}.", currencies.Count, slotStart);
    }

    private static DateTime ResolveCurrentSlotStart(DateTime now)
    {
        var today = now.Date;

        for (var i = SlotsUtc.Length - 1; i >= 0; i--)
        {
            if (now.Hour >= SlotsUtc[i])
                return DateTime.SpecifyKind(today.AddHours(SlotsUtc[i]), DateTimeKind.Utc);
        }

        return DateTime.SpecifyKind(today.AddDays(-1).AddHours(SlotsUtc[^1]), DateTimeKind.Utc);
    }
}
