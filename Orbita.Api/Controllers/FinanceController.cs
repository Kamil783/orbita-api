using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Contracts.ApiDto.Finance.Requests;
using Orbita.Contracts.ApiDto.Finance.Responses;

namespace Orbita.Api.Controllers;

[Route("api/[controller]")]
public class FinanceController(IFinanceService financeService) : AuthorizedControllerBase
{
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.GetBalanceAsync(userId, ct);

        return res
            .Map(b => new BalanceResponse { Balance = b.Balance })
            .ToActionResult(HttpContext);
    }

    [HttpGet("balance/previous-month")]
    public async Task<IActionResult> GetPreviousMonthBalance(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.GetPreviousMonthBalanceAsync(userId, ct);

        return res
            .Map(b => new BalanceResponse { Balance = b.PreviousMonthBalance })
            .ToActionResult(HttpContext);
    }

    [HttpPatch("balance")]
    public async Task<IActionResult> AdjustBalance([FromBody] AdjustBalanceRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.AdjustBalanceAsync(userId, request.Amount, ct);

        return res
            .Map(b => new BalanceResponse { Balance = b.Balance })
            .ToActionResult(HttpContext);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.GetCategoriesAsync(userId, ct);

        return res
            .Map(cats => cats.Select(c => new CategoryResponse
            {
                Id = c.Id.Id.ToString(),
                Name = c.Name,
                Icon = c.Icon,
                Bg = c.Bg,
                Color = c.Color,
                WeeklyLimit = c.WeeklyLimit,
                MonthlyLimit = c.MonthlyLimit
            }).ToList())
            .ToActionResult(HttpContext);
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.CreateCategoryAsync(
            userId, request.Name, request.Icon, request.Bg, request.Color,
            request.WeeklyLimit, request.MonthlyLimit, ct);

        return res
            .Map(c => new CategoryResponse
            {
                Id = c.Id.Id.ToString(),
                Name = c.Name,
                Icon = c.Icon,
                Bg = c.Bg,
                Color = c.Color,
                WeeklyLimit = c.WeeklyLimit,
                MonthlyLimit = c.MonthlyLimit
            })
            .ToActionResult(HttpContext);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.GetTransactionsAsync(userId, ct);

        return res
            .Map(txs => txs.Select(t => new TransactionResponse
            {
                Id = t.Id.Id.ToString(),
                CategoryId = t.CategoryId?.Id.ToString(),
                Title = t.Title,
                Date = t.CreatedAt.ToString("yyyy-MM-dd"),
                Amount = t.Amount,
                Timestamp = new DateTimeOffset(t.CreatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds()
            }).ToList())
            .ToActionResult(HttpContext);
    }

    [HttpPost("transactions")]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        Guid? categoryId = null;
        if (!string.IsNullOrEmpty(request.CategoryId))
        {
            if (!Guid.TryParse(request.CategoryId, out var parsedCategoryId))
                return BadRequest("Invalid category id.");
            categoryId = parsedCategoryId;
        }

        var res = await financeService.CreateTransactionAsync(userId, categoryId, request.Title, request.Amount, request.FromBalance, ct);

        return res
            .Map(t => new TransactionResponse
            {
                Id = t.Id.Id.ToString(),
                CategoryId = t.CategoryId?.Id.ToString(),
                Title = t.Title,
                Date = t.CreatedAt.ToString("yyyy-MM-dd"),
                Amount = t.Amount,
                Timestamp = new DateTimeOffset(t.CreatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds()
            })
            .ToActionResult(HttpContext);
    }

    [HttpPatch("transactions/{id}")]
    public async Task<IActionResult> UpdateTransaction([FromRoute] Guid id, [FromBody] UpdateTransactionRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        Guid? categoryId = null;
        if (request.CategoryId is not null)
        {
            if (!Guid.TryParse(request.CategoryId, out var parsedCategoryId))
                return BadRequest("Invalid category id.");
            categoryId = parsedCategoryId;
        }

        var res = await financeService.UpdateTransactionAsync(userId, id, categoryId, request.Title, request.Amount, ct);

        return res
            .Map(t => new TransactionResponse
            {
                Id = t.Id.Id.ToString(),
                CategoryId = t.CategoryId?.Id.ToString(),
                Title = t.Title,
                Date = t.CreatedAt.ToString("yyyy-MM-dd"),
                Amount = t.Amount,
                Timestamp = new DateTimeOffset(t.CreatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds()
            })
            .ToActionResult(HttpContext);
    }

    [HttpDelete("transactions/{id}")]
    public async Task<IActionResult> DeleteTransaction([FromRoute] Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.DeleteTransactionAsync(userId, id, ct);

        return res.ToActionResult(HttpContext);
    }

    [HttpGet("savings-goals")]
    public async Task<IActionResult> GetSavingsGoals(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.GetSavingsGoalsAsync(userId, ct);

        return res
            .Map(goals => goals.Select(g => new SavingsGoalResponse
            {
                Id = g.Id.Id.ToString(),
                Name = g.Name,
                Target = g.Target,
                Current = g.Current
            }).ToList())
            .ToActionResult(HttpContext);
    }

    [HttpPost("savings-goals")]
    public async Task<IActionResult> CreateSavingsGoal([FromBody] CreateSavingsGoalRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.CreateSavingsGoalAsync(userId, request.Name, request.Target, ct);

        return res
            .Map(g => new SavingsGoalResponse
            {
                Id = g.Id.Id.ToString(),
                Name = g.Name,
                Target = g.Target,
                Current = g.Current
            })
            .ToActionResult(HttpContext);
    }

    [HttpPatch("savings-goals/{id}")]
    public async Task<IActionResult> TopUpSavingsGoal([FromRoute] Guid id, [FromBody] TopUpSavingsGoalRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.TopUpSavingsGoalAsync(userId, id, request.Amount, ct);

        return res
            .Map(g => new SavingsGoalResponse
            {
                Id = g.Id.Id.ToString(),
                Name = g.Name,
                Target = g.Target,
                Current = g.Current
            })
            .ToActionResult(HttpContext);
    }

    [HttpDelete("savings-goals/{id}")]
    public async Task<IActionResult> DeleteSavingsGoal([FromRoute] Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.DeleteSavingsGoalAsync(userId, id, ct);

        return res.ToActionResult(HttpContext);
    }

    [HttpGet("limits")]
    public async Task<IActionResult> GetLimits(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.GetSpendingLimitsAsync(userId, ct);

        return res
            .Map(l => new SpendingLimitsResponse
            {
                MonthlyLimit = l.MonthlyLimit,
                WeeklyLimit = l.WeeklyLimit
            })
            .ToActionResult(HttpContext);
    }

    [HttpPut("limits")]
    public async Task<IActionResult> UpdateLimits([FromBody] UpdateSpendingLimitsRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.UpdateSpendingLimitsAsync(userId, request.MonthlyLimit, request.WeeklyLimit, ct);

        return res
            .Map(l => new SpendingLimitsResponse
            {
                MonthlyLimit = l.MonthlyLimit,
                WeeklyLimit = l.WeeklyLimit
            })
            .ToActionResult(HttpContext);
    }

    [HttpGet("chart-data")]
    public async Task<IActionResult> GetChartData([FromQuery] string period, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.GetChartDataAsync(userId, period, ct);

        return res
            .Map(data => data.Select(d => new ChartDataPointResponse
            {
                Label = d.Label,
                Value = d.Value
            }).ToList())
            .ToActionResult(HttpContext);
    }
}
