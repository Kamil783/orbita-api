using Microsoft.AspNetCore.Mvc;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Contracts.ApiDto.Wallet.Responses;
using Orbita.Domain.Enums;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
public class CurrenciesController(ICurrencyRepository currencyRepository) : AuthorizedControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        if (!TryGetUserId(out _))
            return Unauthorized();

        var currencies = await currencyRepository.GetAllAsync(ct);

        var response = currencies.Select(c => new CurrencyResponse
        {
            Code = c.Code,
            Name = c.Name,
            NumCode = c.NumCode,
            Kind = c.Kind == CurrencyKind.Crypto ? "crypto" : "fiat",
            RateToRub = c.RateToRub,
            Nominal = c.Nominal,
            RateFetchedAt = c.RateFetchedAt.HasValue
                ? new DateTimeOffset(DateTime.SpecifyKind(c.RateFetchedAt.Value, DateTimeKind.Utc), TimeSpan.Zero).ToUnixTimeMilliseconds()
                : null
        }).ToList();

        return Ok(response);
    }
}
