using Microsoft.AspNetCore.Mvc;
using Orbita.Api.Extensions;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Helpers;
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

    [HttpPatch("categories/{id}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.UpdateCategoryAsync(
             userId, id, request.Name, request.Icon, request.Bg, request.Color,
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
                Timestamp = new DateTimeOffset(t.CreatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds(),
                TransactionType = FinanceTransactionHelper.ResolveTransactionType(t, userId)
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

        var res = await financeService.CreateTransactionAsync(userId, categoryId, request.Title, request.Amount, request.FromBalance, request.Date, ct);

        return res
            .Map(t => new TransactionResponse
            {
                Id = t.Id.Id.ToString(),
                CategoryId = t.CategoryId?.Id.ToString(),
                Title = t.Title,
                Date = t.CreatedAt.ToString("yyyy-MM-dd"),
                Amount = t.Amount,
                Timestamp = new DateTimeOffset(t.CreatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds(),
                TransactionType = FinanceTransactionHelper.ResolveTransactionType(t, userId)
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

        var res = await financeService.UpdateTransactionAsync(userId, id, categoryId, request.Title, request.Amount, request.FromBalance, request.Date, ct);

        return res
            .Map(t => new TransactionResponse
            {
                Id = t.Id.Id.ToString(),
                CategoryId = t.CategoryId?.Id.ToString(),
                Title = t.Title,
                Date = t.CreatedAt.ToString("yyyy-MM-dd"),
                Amount = t.Amount,
                Timestamp = new DateTimeOffset(t.CreatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds(),
                TransactionType = FinanceTransactionHelper.ResolveTransactionType(t, userId)
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

    [HttpPatch("savings-goals/{id}/details")]
    public async Task<IActionResult> UpdateSavingsGoalDetails([FromRoute] Guid id, [FromBody] UpdateSavingsGoalRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.UpdateSavingsGoalDetailsAsync(userId, id, request.Name, request.Target, ct);

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

    [HttpPatch("savings-goals/{id}/withdraw")]
    public async Task<IActionResult> WithdrawFromSavingsGoal([FromRoute] Guid id, [FromBody] WithdrawSavingsGoalRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.WithdrawFromSavingsGoalAsync(userId, id, request.Amount, ct);

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

    [HttpGet("shopping-lists")]
    public async Task<IActionResult> GetShoppingLists(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.GetShoppingListsAsync(userId, ct);

        return res
            .Map(lists => lists.Select(l => new ShoppingListResponse
            {
                Id = l.Id.Id.ToString(),
                Name = l.Name,
                ListType = ShoppingListHelper.ResolveListType(l, userId),
                Pinned = l.Pinned,
                CreatedAt = new DateTimeOffset(l.CreatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds(),
                Items = l.Items.Select(i => new ShoppingListItemResponse
                {
                    Id = i.Id.Id.ToString(),
                    Name = i.Name,
                    Price = i.Price,
                    Bought = i.Bought,
                    Order = i.Order
                }).ToList()
            }).ToList())
            .ToActionResult(HttpContext);
    }

    [HttpPost("shopping-lists")]
    public async Task<IActionResult> CreateShoppingList([FromBody] CreateShoppingListRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.CreateShoppingListAsync(userId, request.Name, request.FromBalance, ct);

        return res
            .Map(l => new ShoppingListResponse
            {
                Id = l.Id.Id.ToString(),
                Name = l.Name,
                ListType = ShoppingListHelper.ResolveListType(l, userId),
                Pinned = l.Pinned,
                CreatedAt = new DateTimeOffset(l.CreatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds(),
                Items = l.Items.Select(i => new ShoppingListItemResponse
                {
                    Id = i.Id.Id.ToString(),
                    Name = i.Name,
                    Price = i.Price,
                    Bought = i.Bought,
                    Order = i.Order
                }).ToList()
            })
            .ToActionResult(HttpContext);
    }

    [HttpPatch("shopping-lists/{id}")]
    public async Task<IActionResult> UpdateShoppingList([FromRoute] Guid id, [FromBody] UpdateShoppingListRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.UpdateShoppingListAsync(userId, id, request.Name, request.Pinned, request.FromBalance, ct);

        return res
            .Map(l => new ShoppingListResponse
            {
                Id = l.Id.Id.ToString(),
                Name = l.Name,
                ListType = ShoppingListHelper.ResolveListType(l, userId),
                Pinned = l.Pinned,
                CreatedAt = new DateTimeOffset(l.CreatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds(),
                Items = l.Items.Select(i => new ShoppingListItemResponse
                {
                    Id = i.Id.Id.ToString(),
                    Name = i.Name,
                    Price = i.Price,
                    Bought = i.Bought,
                    Order = i.Order
                }).ToList()
            })
            .ToActionResult(HttpContext);
    }

    [HttpDelete("shopping-lists/{id}")]
    public async Task<IActionResult> DeleteShoppingList([FromRoute] Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.DeleteShoppingListAsync(userId, id, ct);

        return res.ToActionResult(HttpContext);
    }

    [HttpPost("shopping-lists/{listId}/items")]
    public async Task<IActionResult> AddShoppingListItem([FromRoute] Guid listId, [FromBody] AddShoppingListItemRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.AddShoppingListItemAsync(userId, listId, request.Name, request.Price, ct);

        return res
            .Map(i => new ShoppingListItemResponse
            {
                Id = i.Id.Id.ToString(),
                Name = i.Name,
                Price = i.Price,
                Bought = i.Bought,
                Order = i.Order
            })
            .ToActionResult(HttpContext);
    }

    [HttpDelete("shopping-lists/{listId}/items/{itemId}")]
    public async Task<IActionResult> DeleteShoppingListItem([FromRoute] Guid listId, [FromRoute] Guid itemId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.DeleteShoppingListItemAsync(userId, listId, itemId, ct);

        return res.ToActionResult(HttpContext);
    }

    [HttpPatch("shopping-lists/{listId}/items/{itemId}")]
    public async Task<IActionResult> UpdateShoppingListItem([FromRoute] Guid listId, [FromRoute] Guid itemId, [FromBody] UpdateShoppingListItemRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.UpdateShoppingListItemAsync(userId, listId, itemId, request.Bought, ct);

        return res
            .Map(i => new ShoppingListItemResponse
            {
                Id = i.Id.Id.ToString(),
                Name = i.Name,
                Price = i.Price,
                Bought = i.Bought,
                Order = i.Order
            })
            .ToActionResult(HttpContext);
    }

    [HttpPatch("shopping-lists/{listId}/items/{itemId}/details")]
    public async Task<IActionResult> UpdateShoppingListItemDetails([FromRoute] Guid listId, [FromRoute] Guid itemId, [FromBody] UpdateShoppingListItemDetailsRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.UpdateShoppingListItemDetailsAsync(userId, listId, itemId, request.Name, request.Price, ct);

        return res
            .Map(i => new ShoppingListItemResponse
            {
                Id = i.Id.Id.ToString(),
                Name = i.Name,
                Price = i.Price,
                Bought = i.Bought,
                Order = i.Order
            })
            .ToActionResult(HttpContext);
    }

    [HttpPut("shopping-lists/{listId}/items/reorder")]
    public async Task<IActionResult> ReorderShoppingListItems([FromRoute] Guid listId, [FromBody] ReorderShoppingListItemsRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.ReorderShoppingListItemsAsync(userId, listId, request.ItemIds, ct);

        return res.ToActionResult(HttpContext);
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

    [HttpGet("recurring-payments")]
    public async Task<IActionResult> GetRecurringPayments(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.GetRecurringPaymentsAsync(userId, ct);

        return res
            .Map(payments => payments.Select(ToRecurringPaymentResponse).ToList())
            .ToActionResult(HttpContext);
    }

    [HttpPost("recurring-payments")]
    public async Task<IActionResult> CreateRecurringPayment(
        [FromBody] CreateRecurringPaymentRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.CreateRecurringPaymentAsync(
            userId, request.Title, request.Amount, request.DayOfMonth, request.CategoryId, ct);

        return res
            .Map(ToRecurringPaymentResponse)
            .ToActionResult(HttpContext);
    }

    [HttpPatch("recurring-payments/{id:guid}")]
    public async Task<IActionResult> UpdateRecurringPayment(
        Guid id, [FromBody] UpdateRecurringPaymentRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.UpdateRecurringPaymentAsync(
            userId, id, request.Title, request.Amount, request.DayOfMonth,
            request.CategoryId, request.ClearCategory, ct);

        return res
            .Map(ToRecurringPaymentResponse)
            .ToActionResult(HttpContext);
    }

    [HttpDelete("recurring-payments/{id:guid}")]
    public async Task<IActionResult> DeleteRecurringPayment(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.DeleteRecurringPaymentAsync(userId, id, ct);

        return res.ToActionResult(HttpContext);
    }

    [HttpGet("planned-purchases")]
    public async Task<IActionResult> GetPlannedPurchases(
        [FromQuery] string? from,
        [FromQuery] string? to,
        [FromQuery] string? status,
        [FromQuery] string? assigneeKind,
        [FromQuery] Guid? assigneeUserId,
        [FromQuery] Guid? categoryId,
        CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        DateOnly? fromDate = null;
        if (!string.IsNullOrWhiteSpace(from))
        {
            if (!DateOnly.TryParse(from, out var parsed))
                return BadRequest("Invalid 'from' date.");
            fromDate = parsed;
        }

        DateOnly? toDate = null;
        if (!string.IsNullOrWhiteSpace(to))
        {
            if (!DateOnly.TryParse(to, out var parsed))
                return BadRequest("Invalid 'to' date.");
            toDate = parsed;
        }

        Domain.Enums.PlannedPurchaseStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!TryParsePlannedPurchaseStatus(status, out var parsed))
                return BadRequest("Invalid status.");
            statusFilter = parsed;
        }

        Domain.Enums.PlannedPurchaseAssigneeKind? kindFilter = null;
        if (!string.IsNullOrWhiteSpace(assigneeKind))
        {
            if (!TryParseAssigneeKind(assigneeKind, out var parsed))
                return BadRequest("Invalid assigneeKind.");
            kindFilter = parsed;
        }

        var res = await financeService.GetPlannedPurchasesAsync(
            userId, fromDate, toDate, statusFilter, kindFilter, assigneeUserId, categoryId, ct);

        return res
            .Map(items => items.Select(ToPlannedPurchaseResponse).ToList())
            .ToActionResult(HttpContext);
    }

    [HttpPost("planned-purchases")]
    public async Task<IActionResult> CreatePlannedPurchase(
        [FromBody] CreatePlannedPurchaseRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        if (!DateOnly.TryParse(request.Date, out var date))
            return BadRequest("Invalid date.");

        Domain.Enums.PlannedPurchaseAssigneeKind? kind = null;
        if (request.AssigneeKind is not null)
        {
            if (!TryParseAssigneeKind(request.AssigneeKind, out var parsed))
                return BadRequest("Invalid assigneeKind.");
            kind = parsed;
        }

        var res = await financeService.CreatePlannedPurchaseAsync(
            userId,
            request.Title,
            date,
            request.Amount,
            kind,
            request.AssigneeUserId,
            request.CategoryId,
            request.Note,
            ct);

        return res
            .Map(ToPlannedPurchaseResponse)
            .ToActionResult(HttpContext);
    }

    [HttpPatch("planned-purchases/{id:guid}")]
    public async Task<IActionResult> UpdatePlannedPurchase(
        Guid id, [FromBody] UpdatePlannedPurchaseRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        DateOnly? date = null;
        if (request.Date is not null)
        {
            if (!DateOnly.TryParse(request.Date, out var parsed))
                return BadRequest("Invalid date.");
            date = parsed;
        }

        Domain.Enums.PlannedPurchaseStatus? status = null;
        if (request.Status is not null)
        {
            if (!TryParsePlannedPurchaseStatus(request.Status, out var parsed))
                return BadRequest("Invalid status.");
            status = parsed;
        }

        Domain.Enums.PlannedPurchaseAssigneeKind? kind = null;
        if (request.AssigneeKind is not null)
        {
            if (!TryParseAssigneeKind(request.AssigneeKind, out var parsed))
                return BadRequest("Invalid assigneeKind.");
            kind = parsed;
        }

        var res = await financeService.UpdatePlannedPurchaseAsync(
            userId,
            id,
            request.Title,
            date,
            request.Amount,
            kind,
            request.AssigneeUserId,
            request.CategoryId,
            request.Note,
            status,
            ct);

        return res
            .Map(ToPlannedPurchaseResponse)
            .ToActionResult(HttpContext);
    }

    [HttpDelete("planned-purchases/{id:guid}")]
    public async Task<IActionResult> DeletePlannedPurchase(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var res = await financeService.DeletePlannedPurchaseAsync(userId, id, ct);
        return res.ToActionResult(HttpContext);
    }

    private static bool TryParsePlannedPurchaseStatus(string raw, out Domain.Enums.PlannedPurchaseStatus status)
    {
        switch (raw.Trim().ToLowerInvariant())
        {
            case "planned": status = Domain.Enums.PlannedPurchaseStatus.Planned; return true;
            case "bought": status = Domain.Enums.PlannedPurchaseStatus.Bought; return true;
            case "cancelled": status = Domain.Enums.PlannedPurchaseStatus.Cancelled; return true;
            default: status = default; return false;
        }
    }

    private static string PlannedPurchaseStatusToString(Domain.Enums.PlannedPurchaseStatus s) => s switch
    {
        Domain.Enums.PlannedPurchaseStatus.Planned => "planned",
        Domain.Enums.PlannedPurchaseStatus.Bought => "bought",
        Domain.Enums.PlannedPurchaseStatus.Cancelled => "cancelled",
        _ => "planned"
    };

    private static bool TryParseAssigneeKind(string raw, out Domain.Enums.PlannedPurchaseAssigneeKind kind)
    {
        switch (raw.Trim().ToLowerInvariant())
        {
            case "user": kind = Domain.Enums.PlannedPurchaseAssigneeKind.User; return true;
            case "team": kind = Domain.Enums.PlannedPurchaseAssigneeKind.Team; return true;
            default: kind = default; return false;
        }
    }

    private static string? AssigneeKindToString(Domain.Enums.PlannedPurchaseAssigneeKind? k) => k switch
    {
        Domain.Enums.PlannedPurchaseAssigneeKind.User => "user",
        Domain.Enums.PlannedPurchaseAssigneeKind.Team => "team",
        _ => null
    };

    private static PlannedPurchaseResponse ToPlannedPurchaseResponse(Domain.Entities.PlannedPurchase p) =>
        new()
        {
            Id = p.Id.Id.ToString(),
            Title = p.Title,
            Date = p.Date.ToString("yyyy-MM-dd"),
            Amount = p.Amount,
            AssigneeKind = AssigneeKindToString(p.AssigneeKind),
            AssigneeUserId = p.AssigneeUserId?.Id.ToString(),
            CategoryId = p.CategoryId?.Id.ToString(),
            Note = p.Note,
            Status = PlannedPurchaseStatusToString(p.Status),
            CreatedAt = new DateTimeOffset(p.CreatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds(),
            UpdatedAt = new DateTimeOffset(p.UpdatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds()
        };

    private static RecurringPaymentResponse ToRecurringPaymentResponse(Domain.Entities.RecurringPayment p) =>
        new()
        {
            Id = p.Id.Id.ToString(),
            Title = p.Title,
            Amount = p.Amount,
            DayOfMonth = p.DayOfMonth,
            CategoryId = p.CategoryId?.Id.ToString(),
            CreatedAt = new DateTimeOffset(p.CreatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds(),
            UpdatedAt = new DateTimeOffset(p.UpdatedAt, TimeSpan.Zero).ToUnixTimeMilliseconds()
        };
}
