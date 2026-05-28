using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.Wallet.Requests;
using Orbita.Contracts.ApiDto.Wallet.Responses;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
public class AccountsController(IAccountService accountService) : AuthorizedControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await accountService.GetAsync(userId, ct);

        return res
            .Map(items => items.Select(ToResponse).ToList())
            .ToActionResult(HttpContext);
    }

    [HttpGet("total")]
    public async Task<IActionResult> GetTotal(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await accountService.GetTotalAsync(userId, ct);

        return res
            .Map(t => new AccountsTotalResponse
            {
                TotalRub = t.TotalRub,
                Items = t.Items.Select(i => new AccountTotalItemResponse
                {
                    Id = i.Account.Id.Id.ToString(),
                    Name = i.Account.Name,
                    CurrencyCode = i.Account.CurrencyCode,
                    Balance = i.Account.Balance,
                    ConvertedRub = i.ConvertedRub
                }).ToList()
            })
            .ToActionResult(HttpContext);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await accountService.CreateAsync(userId, request.Name, request.CurrencyCode, request.Balance, ct);

        return res
            .Map(ToResponse)
            .ToActionResult(HttpContext);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAccountRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await accountService.UpdateAsync(
            userId, id, request.Name, request.CurrencyCode, request.Balance, ct);

        return res
            .Map(ToResponse)
            .ToActionResult(HttpContext);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await accountService.DeleteAsync(userId, id, ct);
        return res.ToActionResult(HttpContext);
    }

    private static AccountResponse ToResponse(Domain.Entities.Account a) => new()
    {
        Id = a.Id.Id.ToString(),
        Name = a.Name,
        CurrencyCode = a.CurrencyCode,
        Balance = a.Balance,
        CreatedAt = new DateTimeOffset(a.CreatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds(),
        UpdatedAt = new DateTimeOffset(a.UpdatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds()
    };
}
