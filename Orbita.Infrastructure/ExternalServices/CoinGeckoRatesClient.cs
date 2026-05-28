using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Orbita.Infrastructure.ExternalServices;

/// <summary>Описание поддерживаемой монеты: тикер для нашей БД, ID на CoinGecko, human-readable name.</summary>
public sealed record CoinDescriptor(string Ticker, string CoinGeckoId, string Name);

/// <summary>Курс крипты к рублю на конкретный момент.</summary>
public sealed record CryptoRate(string Ticker, string Name, decimal RatePerUnitRub);

public class CoinGeckoRatesClient(HttpClient httpClient, ILogger<CoinGeckoRatesClient> logger)
{
    /// <summary>
    /// Поддерживаемые криптовалюты. Добавление новой монеты = вписать строку сюда + (опционально)
    /// пересоздать запись в БД, иначе она появится автоматически при следующем фетче job'а.
    /// </summary>
    public static readonly IReadOnlyList<CoinDescriptor> SupportedCoins =
    [
        new("BTC",  "bitcoin",  "Bitcoin"),
        new("ETH",  "ethereum", "Ethereum"),
        new("USDT", "tether",   "Tether"),
        new("SOL",  "solana",   "Solana")
    ];

    private const string Endpoint = "https://api.coingecko.com/api/v3/simple/price";

    public async Task<IReadOnlyList<CryptoRate>> FetchAsync(CancellationToken ct)
    {
        var ids = string.Join(',', SupportedCoins.Select(c => c.CoinGeckoId));
        var url = $"{Endpoint}?ids={ids}&vs_currencies=rub";

        logger.LogInformation("CoinGecko: requesting {Url}", url);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        // Некоторые CDN-фронты CoinGecko отбивают запросы без UA.
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Orbita", "1.0"));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await SafeReadBody(response, ct);
            logger.LogError(
                "CoinGecko HTTP {Status} {Reason}. Body: {Body}",
                (int)response.StatusCode, response.ReasonPhrase, body);
            response.EnsureSuccessStatusCode();
        }

        // Ответ: { "bitcoin": { "rub": 5823000 }, "ethereum": { "rub": 245300 }, ... }
        var payload = await response.Content
            .ReadFromJsonAsync<Dictionary<string, Dictionary<string, decimal>>>(ct);

        if (payload is null || payload.Count == 0)
        {
            var body = await SafeReadBody(response, ct);
            logger.LogWarning("CoinGecko returned empty/null payload. Raw body: {Body}", body);
            return Array.Empty<CryptoRate>();
        }

        var result = new List<CryptoRate>(SupportedCoins.Count);

        foreach (var coin in SupportedCoins)
        {
            if (!payload.TryGetValue(coin.CoinGeckoId, out var prices) ||
                !prices.TryGetValue("rub", out var rate) || rate <= 0)
            {
                logger.LogWarning(
                    "CoinGecko: rate for {Coin} (id={Id}) missing or invalid.",
                    coin.Ticker, coin.CoinGeckoId);
                continue;
            }

            result.Add(new CryptoRate(coin.Ticker, coin.Name, rate));
        }

        logger.LogInformation("CoinGecko: parsed {Count} of {Expected} coins.",
            result.Count, SupportedCoins.Count);

        return result;
    }

    private static async Task<string> SafeReadBody(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            // ReadFromJsonAsync уже мог считать поток. Перечитываем через буфер ContentReadStream.
            var bytes = await response.Content.ReadAsByteArrayAsync(ct);
            var text = System.Text.Encoding.UTF8.GetString(bytes);
            return text.Length > 2000 ? text[..2000] + "…(truncated)" : text;
        }
        catch
        {
            return "<unreadable>";
        }
    }
}
