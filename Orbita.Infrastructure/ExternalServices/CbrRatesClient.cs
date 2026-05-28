using System.Globalization;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace Orbita.Infrastructure.ExternalServices;

/// <summary>Запись курса из ответа CBR (https://www.cbr-xml-daily.ru/daily_utf8.xml).</summary>
public sealed record CbrRate(
    string CharCode,
    string Name,
    int? NumCode,
    int Nominal,
    decimal RatePerNominalRub);

public class CbrRatesClient(HttpClient httpClient, ILogger<CbrRatesClient> logger)
{
    private const string Endpoint = "https://www.cbr-xml-daily.ru/daily_utf8.xml";

    public async Task<IReadOnlyList<CbrRate>> FetchAsync(CancellationToken ct)
    {
        using var response = await httpClient.GetAsync(Endpoint, ct);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        var doc = await XDocument.LoadAsync(stream, LoadOptions.None, ct);

        var root = doc.Root
            ?? throw new InvalidOperationException("CBR response has no root element.");

        var result = new List<CbrRate>();

        foreach (var valute in root.Elements("Valute"))
        {
            var charCode = valute.Element("CharCode")?.Value?.Trim();
            var name = valute.Element("Name")?.Value?.Trim();
            var nominalRaw = valute.Element("Nominal")?.Value;
            // Value использует запятую как десятичный разделитель, VunitRate — точку и уже разделён на номинал.
            // Берём Value + Nominal, чтобы сохранить «честный» масштаб.
            var valueRaw = valute.Element("Value")?.Value;
            var numCodeRaw = valute.Element("NumCode")?.Value;

            if (string.IsNullOrWhiteSpace(charCode) || string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(nominalRaw) || string.IsNullOrWhiteSpace(valueRaw))
            {
                logger.LogWarning("Skipping malformed CBR Valute entry: {Raw}", valute);
                continue;
            }

            if (!int.TryParse(nominalRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var nominal) ||
                nominal <= 0)
            {
                logger.LogWarning("Skipping CBR Valute {Code} with invalid Nominal '{Nominal}'", charCode, nominalRaw);
                continue;
            }

            if (!decimal.TryParse(valueRaw.Replace(',', '.'),
                    NumberStyles.Number, CultureInfo.InvariantCulture, out var value) ||
                value <= 0)
            {
                logger.LogWarning("Skipping CBR Valute {Code} with invalid Value '{Value}'", charCode, valueRaw);
                continue;
            }

            int? numCode = null;
            if (int.TryParse(numCodeRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedNum))
                numCode = parsedNum;

            result.Add(new CbrRate(
                CharCode: charCode.ToUpperInvariant(),
                Name: name,
                NumCode: numCode,
                Nominal: nominal,
                RatePerNominalRub: value));
        }

        return result;
    }
}
