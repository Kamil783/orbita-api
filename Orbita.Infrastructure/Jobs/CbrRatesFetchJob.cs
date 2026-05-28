using Microsoft.Extensions.Logging;
using Orbita.Application.Abstractions.Jobs;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Infrastructure.ExternalServices;

namespace Orbita.Infrastructure.Jobs;

/// <summary>
/// Тянет курсы ЦБ РФ из cbr-xml-daily.ru три раза в сутки (07:00 / 14:00 / 20:00 UTC).
/// DailyTaskRunnerService запускает все IDailyJob'ы каждый час, поэтому здесь логика
/// «гейта»: фетчим только если текущий слот ещё не закрыт (LastFetchedAt &lt; slotStart).
/// </summary>
public class CbrRatesFetchJob(
    CbrRatesClient cbrClient,
    ICurrencyRepository currencyRepository,
    ILogger<CbrRatesFetchJob> logger) : IDailyJob
{
    private static readonly int[] SlotsUtc = [7, 14, 20];

    public string Name => "CbrRatesFetch";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var slotStart = ResolveCurrentSlotStart(now);

        var lastFetchedAt = await currencyRepository.GetLatestFetchedAtAsync(CurrencyKind.Fiat, ct);
        if (lastFetchedAt is not null && lastFetchedAt.Value >= slotStart)
        {
            // Уже фетчили в этом слоте — пропускаем.
            return;
        }

        IReadOnlyList<CbrRate> rates;
        try
        {
            rates = await cbrClient.FetchAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CBR rates fetch failed");
            return;
        }

        if (rates.Count == 0)
        {
            logger.LogWarning("CBR returned no rates; skipping upsert.");
            return;
        }

        var fetchedAt = DateTime.UtcNow;
        var currencies = rates.Select(r =>
        {
            var c = Currency.Create(r.CharCode, r.Name, r.NumCode, CurrencyKind.Fiat);
            c.UpdateRate(r.RatePerNominalRub, r.Nominal, fetchedAt);
            return c;
        }).ToList();

        await currencyRepository.UpsertManyAsync(currencies, ct);

        logger.LogInformation(
            "CBR rates updated: {Count} currencies for slot {Slot:O}.", currencies.Count, slotStart);
    }

    /// <summary>Возвращает старт «текущего» слота — последний из 7/14/20 UTC, который &lt;= now.
    /// Если now &lt; 07:00, берём вчерашний 20:00 слот.</summary>
    private static DateTime ResolveCurrentSlotStart(DateTime now)
    {
        var today = now.Date;

        for (var i = SlotsUtc.Length - 1; i >= 0; i--)
        {
            if (now.Hour >= SlotsUtc[i])
                return DateTime.SpecifyKind(today.AddHours(SlotsUtc[i]), DateTimeKind.Utc);
        }

        // Раньше 07:00 — текущим слотом считаем вчерашний 20:00.
        return DateTime.SpecifyKind(today.AddDays(-1).AddHours(SlotsUtc[^1]), DateTimeKind.Utc);
    }
}
