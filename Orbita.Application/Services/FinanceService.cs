using Orbita.Application.Abstractions;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;
using System.Globalization;
using System.Transactions;

namespace Orbita.Application.Services;

public class FinanceService(
    IFinanceBalanceRepository balanceRepository,
    IFinanceCategoryRepository categoryRepository,
    IFinanceTransactionRepository transactionRepository,
    ISavingsGoalRepository savingsGoalRepository,
    ISpendingLimitRepository spendingLimitRepository,
    IShoppingListRepository shoppingListRepository,
    IRecurringPaymentRepository recurringPaymentRepository,
    IPlannedPurchaseRepository plannedPurchaseRepository,
    ITeamRepository teamRepository,
    ITeamProvider teamProvider,
    INotificationDispatcher notificationDispatcher,
    IUnitOfWork unitOfWork) : IFinanceService
{
    public async Task<Result<FinanceBalance>> GetBalanceAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var balance = await balanceRepository.GetAsync(teamId, ct);
        if (balance is null)
        {
            balance = FinanceBalance.Create(new TeamId(teamId));
            balance = await balanceRepository.CreateAsync(balance, ct);
        }

        return Result<FinanceBalance>.Ok(balance);
    }

    public async Task<Result<FinanceBalance>> GetPreviousMonthBalanceAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var balance = await balanceRepository.GetAsync(teamId, ct);
        if (balance is null)
        {
            balance = FinanceBalance.Create(new TeamId(teamId));
            balance = await balanceRepository.CreateAsync(balance, ct);
        }

        return Result<FinanceBalance>.Ok(balance);
    }

    public async Task<Result<FinanceBalance>> AdjustBalanceAsync(Guid userId, long amount, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var balance = await balanceRepository.GetAsync(teamId, ct);
        if (balance is null)
        {
            balance = FinanceBalance.Create(new TeamId(teamId));
            balance = await balanceRepository.CreateAsync(balance, ct);
        }

        balance.Adjust(amount);
        var updated = await balanceRepository.UpdateAsync(balance, ct);

        return Result<FinanceBalance>.Ok(updated);
    }

    public async Task<Result<List<FinanceCategory>>> GetCategoriesAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var categories = await categoryRepository.GetByTeamAsync(teamId, ct);
        return Result<List<FinanceCategory>>.Ok(categories);
    }

    public async Task<Result<FinanceCategory>> CreateCategoryAsync(
        Guid userId, string name, string icon, string bg, string color,
        long? weeklyLimit, long? monthlyLimit, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var category = FinanceCategory.Create(
            creatorId: new UserId(userId),
            teamId: new TeamId(teamId),
            name: name,
            icon: icon,
            bg: bg,
            color: color,
            weeklyLimit: weeklyLimit,
            monthlyLimit: monthlyLimit);

        var created = await categoryRepository.CreateAsync(category, ct);
        return Result<FinanceCategory>.Ok(created);
    }

    public async Task<Result<FinanceCategory>> UpdateCategoryAsync(
        Guid userId, Guid categoryId, string? name, string? icon, string? bg, string? color,
        long? weeklyLimit, long? monthlyLimit, CancellationToken ct = default)
    {

        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var category = await categoryRepository.GetAsync(categoryId, ct);
        if (category is null)
            return Result<FinanceCategory>.NotFound("Category not found.");

        if (category.TeamId.Id != teamId)
            return Result<FinanceCategory>.Forbidden("Access denied.");

        if (name is not null)
            category.SetName(name);

        if (icon is not null)
            category.SetIcon(icon);

        if (bg is not null)
            category.SetBg(bg);

        if (color is not null)
            category.SetColor(color);

        if (weeklyLimit.HasValue || monthlyLimit.HasValue)
        {
            category.SetLimits(
                weeklyLimit ?? category.WeeklyLimit,
                monthlyLimit ?? category.MonthlyLimit);
        }

        var updated = await categoryRepository.UpdateAsync(category, ct);
        return Result<FinanceCategory>.Ok(updated);
    }

    public async Task<Result<List<FinanceTransaction>>> GetTransactionsAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var transactions = await transactionRepository.GetForUserAsync(teamId, userId, ct);
        return Result<List<FinanceTransaction>>.Ok(transactions);
    }

    public async Task<Result<FinanceTransaction>> CreateTransactionAsync(
        Guid userId, Guid? categoryId, string title, long amount, bool fromBalance, DateTime? createdAt, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var utc = createdAt.HasValue
           ? DateTime.SpecifyKind(createdAt.Value, DateTimeKind.Utc)
           : DateTime.UtcNow;

        FinanceCategoryId? financeCategoryId = null;
        if (categoryId.HasValue)
        {
            var category = await categoryRepository.GetAsync(categoryId.Value, ct);
            if (category is null)
                return Result<FinanceTransaction>.NotFound("Category not found.");

            financeCategoryId = new FinanceCategoryId(categoryId.Value);
        }

        var transaction = FinanceTransaction.Create(
            creatorId: new UserId(userId),
            teamId: new TeamId(teamId),
            categoryId: financeCategoryId,
            title: title,
            amount: amount,
            isFromBalance: fromBalance,
            createdAt: utc);

        var created = await transactionRepository.CreateAsync(transaction, ct);

        if (fromBalance)
        {
            var balance = await balanceRepository.GetAsync(teamId, ct);
            if (balance is null)
            {
                balance = FinanceBalance.Create(new TeamId(teamId));
                await balanceRepository.CreateAsync(balance, ct);
            }

            balance.Adjust(amount);
            await balanceRepository.UpdateAsync(balance, ct);

            // Командное уведомление: операция с общего баланса. Инициатор не уведомляется.
            var sign = amount < 0 ? "−" : "+";
            var rubles = Math.Abs(amount) / 100m;
            await notificationDispatcher.SendToTeamAsync(
                teamId: teamId,
                type: NotificationType.Finance,
                title: "Операция с общего баланса",
                message: $"{title}: {sign}{rubles:0.00} ₽",
                excludeUserId: userId,
                pushOverHub: true,
                ct: ct);
        }

        return Result<FinanceTransaction>.Ok(created);
    }

    public async Task<Result<FinanceTransaction>> UpdateTransactionAsync(
        Guid userId, Guid transactionId, Guid? categoryId, string? title, long? amount, bool? fromBalance, DateTime? createdAt, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var transaction = await transactionRepository.GetAsync(transactionId, ct);
        if (transaction is null)
            return Result<FinanceTransaction>.NotFound("Transaction not found.");

        if (transaction.TeamId.Id != teamId)
            return Result<FinanceTransaction>.Forbidden("Access denied.");

        var oldAmount = transaction.Amount;
        var balance = await balanceRepository.GetAsync(teamId, ct);

        if (categoryId.HasValue)
        {
            var category = await categoryRepository.GetAsync(categoryId.Value, ct);
            if (category is null)
                return Result<FinanceTransaction>.NotFound("Category not found.");

            transaction.SetCategoryId(new FinanceCategoryId(categoryId.Value));
        }
        else
            transaction.SetNullCategory();

        if (title is not null)
            transaction.SetTitle(title);

        var oldIsFromBalance = transaction.IsFromBalance;
        var newIsFromBalance = fromBalance ?? oldIsFromBalance;
        var newAmount = amount ?? oldAmount;

        if (fromBalance.HasValue)
            transaction.SetIsFromBalance(newIsFromBalance);

        if (amount.HasValue)
            transaction.SetAmount(newAmount);

        var balanceDelta = CalculateBalanceDelta(
            oldIsFromBalance,
            oldAmount,
            newIsFromBalance,
            newAmount);

        if (balanceDelta != 0 && balance is not null)
        {
            balance.Adjust(balanceDelta);
            await balanceRepository.UpdateAsync(balance, ct);
        }

        if (createdAt.HasValue)
        {
            var utc = DateTime.SpecifyKind(createdAt.Value, DateTimeKind.Utc);
            transaction.SetCreatedAt(utc);
        }

        var updated = await transactionRepository.UpdateAsync(transaction, ct);

        return Result<FinanceTransaction>.Ok(updated!);
    }

    public async Task<Result> DeleteTransactionAsync(Guid userId, Guid transactionId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var transaction = await transactionRepository.GetAsync(transactionId, ct);
        if (transaction is null)
            return Result.NotFound("Transaction not found.");

        if (transaction.TeamId.Id != teamId)
            return Result.Forbidden("Access denied.");

        if (transaction.IsFromBalance)
        {
            var balance = await balanceRepository.GetAsync(teamId, ct);
            if (balance is not null)
            {
                balance.Adjust(-transaction.Amount);
                await balanceRepository.UpdateAsync(balance, ct);
            }
        }

        await transactionRepository.DeleteAsync(transactionId, ct);

        return Result.Ok();
    }

    public async Task<Result<List<SavingsGoal>>> GetSavingsGoalsAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var goals = await savingsGoalRepository.GetByTeamAsync(teamId, ct);
        return Result<List<SavingsGoal>>.Ok(goals);
    }

    public async Task<Result<SavingsGoal>> CreateSavingsGoalAsync(Guid userId, string name, long target, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var goal = SavingsGoal.Create(
            creatorId: new UserId(userId),
            teamId: new TeamId(teamId),
            name: name,
            target: target);

        var created = await savingsGoalRepository.CreateAsync(goal, ct);
        return Result<SavingsGoal>.Ok(created);
    }

    public async Task<Result<SavingsGoal>> UpdateSavingsGoalDetailsAsync(Guid userId, Guid goalId, string? name, long? target, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var goal = await savingsGoalRepository.GetAsync(goalId, ct);
        if (goal is null)
            return Result<SavingsGoal>.NotFound("Savings goal not found.");

        if (goal.TeamId.Id != teamId)
            return Result<SavingsGoal>.Forbidden("Access denied.");

        try
        {
            goal.UpdateDetails(name, target);
        }
        catch (Exception ex)
        {
            return Result<SavingsGoal>.Fail(ex.Message, ErrorType.Validation);
        }

        var updated = await savingsGoalRepository.UpdateAsync(goal, ct);
        return Result<SavingsGoal>.Ok(updated);
    }

    public async Task<Result<SavingsGoal>> TopUpSavingsGoalAsync(Guid userId, Guid goalId, long amount, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var goal = await savingsGoalRepository.GetAsync(goalId, ct);
        if (goal is null)
            return Result<SavingsGoal>.NotFound("Savings goal not found.");

        if (goal.TeamId.Id != teamId)
            return Result<SavingsGoal>.Forbidden("Access denied.");

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var balance = await balanceRepository.GetAsync(teamId, ct);
            if (balance is null)
            {
                await unitOfWork.RollbackAsync(ct);
                return Result<SavingsGoal>.Conflict("Balance not found.");
            }

            balance.Adjust(-amount);
            await balanceRepository.UpdateAsync(balance, ct);

            goal.AddFunds(amount);
            var updated = await savingsGoalRepository.UpdateAsync(goal, ct);

            await unitOfWork.CommitAsync(ct);
            return Result<SavingsGoal>.Ok(updated);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<Result<SavingsGoal>> WithdrawFromSavingsGoalAsync(Guid userId, Guid goalId, long amount, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var goal = await savingsGoalRepository.GetAsync(goalId, ct);
        if (goal is null)
            return Result<SavingsGoal>.NotFound("Savings goal not found.");

        if (goal.TeamId.Id != teamId)
            return Result<SavingsGoal>.Forbidden("Access denied.");

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var balance = await balanceRepository.GetAsync(teamId, ct);
            if (balance is null)
            {
                await unitOfWork.RollbackAsync(ct);
                return Result<SavingsGoal>.Conflict("Balance not found.");
            }

            balance.Adjust(amount);
            await balanceRepository.UpdateAsync(balance, ct);

            goal.WithdrawFunds(amount);
            var updated = await savingsGoalRepository.UpdateAsync(goal, ct);

            await unitOfWork.CommitAsync(ct);
            return Result<SavingsGoal>.Ok(updated);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<Result> DeleteSavingsGoalAsync(Guid userId, Guid goalId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var goal = await savingsGoalRepository.GetAsync(goalId, ct);
        if (goal is null)
            return Result.NotFound("Savings goal not found.");

        if (goal.TeamId.Id != teamId)
            return Result.Forbidden("Access denied.");

        await savingsGoalRepository.DeleteAsync(goalId, ct);
        return Result.Ok();
    }

    public async Task<Result<SpendingLimit>> GetSpendingLimitsAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var limit = await spendingLimitRepository.GetAsync(teamId, ct);
        if (limit is null)
        {
            limit = SpendingLimit.Create(new TeamId(teamId));
            limit = await spendingLimitRepository.CreateAsync(limit, ct);
        }

        return Result<SpendingLimit>.Ok(limit);
    }

    public async Task<Result<SpendingLimit>> UpdateSpendingLimitsAsync(
        Guid userId, long monthlyLimit, long weeklyLimit, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var limit = await spendingLimitRepository.GetAsync(teamId, ct);
        if (limit is null)
        {
            limit = SpendingLimit.Create(new TeamId(teamId));
            await spendingLimitRepository.CreateAsync(limit, ct);
        }

        limit.Update(monthlyLimit, weeklyLimit);
        var updated = await spendingLimitRepository.UpdateAsync(limit, ct);

        return Result<SpendingLimit>.Ok(updated);
    }

    public async Task<Result<List<(string Label, decimal Value)>>> GetChartDataAsync(
        Guid userId, string period, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var now = DateTime.UtcNow;
        var culture = new CultureInfo("ru-RU");

        return period switch
        {
            "weekly" => Result<List<(string Label, decimal Value)>>.Ok(
                await GetWeeklyChartDataAsync(teamId, now, ct)),

            "monthly" => Result<List<(string Label, decimal Value)>>.Ok(
                await GetMonthlyChartDataAsync(teamId, now, culture, ct)),

            "yearly" => Result<List<(string Label, decimal Value)>>.Ok(
                await GetYearlyChartDataAsync(teamId, now, culture, ct)),

            _ => Result<List<(string Label, decimal Value)>>.Fail(
                "Invalid period. Use 'weekly', 'monthly' or 'yearly'.")
        };
    }

    public async Task<Result<List<ShoppingList>>> GetShoppingListsAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var lists = await shoppingListRepository.GetForUserAsync(teamId, userId, ct);
        return Result<List<ShoppingList>>.Ok(lists);
    }

    public async Task<Result<ShoppingList>> CreateShoppingListAsync(Guid userId, string name, bool fromBalance, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var list = ShoppingList.Create(
            creatorId: new UserId(userId),
            teamId: new TeamId(teamId),
            name: name,
            isFromBalance: fromBalance);

        var created = await shoppingListRepository.CreateAsync(list, ct);
        return Result<ShoppingList>.Ok(created);
    }

    public async Task<Result<ShoppingList>> UpdateShoppingListAsync(Guid userId, Guid listId, string? name, bool? pinned, bool? isFromBalance, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var list = await shoppingListRepository.GetAsync(listId, ct);
        if (list is null)
            return Result<ShoppingList>.NotFound("Shopping list not found.");

        if (list.TeamId.Id != teamId)
            return Result<ShoppingList>.Forbidden("Access denied.");

        try
        {
            if (name is not null)
                list.SetName(name);
            if (pinned.HasValue)
                list.SetPinned(pinned.Value);
            if (isFromBalance.HasValue)
                list.SetIsFromBalance(isFromBalance.Value);

        }
        catch (Exception ex)
        {
            return Result<ShoppingList>.Fail(ex.Message, ErrorType.Validation);
        }

        var updated = await shoppingListRepository.UpdateAsync(list, ct);
        return Result<ShoppingList>.Ok(updated);
    }

    public async Task<Result> DeleteShoppingListAsync(Guid userId, Guid listId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var list = await shoppingListRepository.GetAsync(listId, ct);
        if (list is null)
            return Result.NotFound("Shopping list not found.");

        if (list.TeamId.Id != teamId)
            return Result.Forbidden("Access denied.");

        await shoppingListRepository.DeleteAsync(listId, ct);
        return Result.Ok();
    }

    public async Task<Result<ShoppingListItem>> AddShoppingListItemAsync(Guid userId, Guid listId, string name, long? price, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var list = await shoppingListRepository.GetAsync(listId, ct);
        if (list is null)
            return Result<ShoppingListItem>.NotFound("Shopping list not found.");

        if (list.TeamId.Id != teamId)
            return Result<ShoppingListItem>.Forbidden("Access denied.");

        var maxOrder = await shoppingListRepository.GetMaxItemOrderAsync(listId, ct);

        var item = ShoppingListItem.Create(
            listId: new ShoppingListId(listId),
            name: name,
            price: price,
            order: maxOrder + 1);

        var created = await shoppingListRepository.AddItemAsync(item, ct);
        return Result<ShoppingListItem>.Ok(created);
    }

    public async Task<Result> DeleteShoppingListItemAsync(Guid userId, Guid listId, Guid itemId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var list = await shoppingListRepository.GetAsync(listId, ct);
        if (list is null)
            return Result.NotFound("Shopping list not found.");

        if (list.TeamId.Id != teamId)
            return Result.Forbidden("Access denied.");

        var item = await shoppingListRepository.GetItemAsync(itemId, ct);
        if (item is null)
            return Result.NotFound("Shopping list item not found.");

        if (item.ListId.Id != listId)
            return Result.NotFound("Shopping list item not found.");

        await shoppingListRepository.DeleteItemAsync(itemId, ct);
        return Result.Ok();
    }

    public async Task<Result<ShoppingListItem>> UpdateShoppingListItemAsync(Guid userId, Guid listId, Guid itemId, bool bought, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var list = await shoppingListRepository.GetAsync(listId, ct);
        if (list is null)
            return Result<ShoppingListItem>.NotFound("Shopping list not found.");

        if (list.TeamId.Id != teamId)
            return Result<ShoppingListItem>.Forbidden("Access denied.");

        var item = await shoppingListRepository.GetItemAsync(itemId, ct);
        if (item is null)
            return Result<ShoppingListItem>.NotFound("Shopping list item not found.");

        if (item.ListId.Id != listId)
            return Result<ShoppingListItem>.NotFound("Shopping list item not found.");

        return await unitOfWork.ExecuteAsync(async token =>
        {
            var delta = item.ChangeBoughtStatus(bought);

            if (bought && item.Price.HasValue)
            {

                var transaction = await CreateTransactionAsync(
                    userId: userId,
                    categoryId: null,
                    title: item.Name,
                    amount: -item.Price.Value,
                    fromBalance: list.IsFromBalance,
                    createdAt: null,
                    ct);

                if(transaction.IsSuccess)
                {
                    item.LinkFinanceTransaction(new FinanceTransactionId(transaction.Value!.Id.Id));
                }
                else
                {
                    return Result<ShoppingListItem>.Fail("Failed to create transaction for the bought item.");
                }
            }
            else
            {
                if (item.FinanceTransactionId is null)
                    return Result<ShoppingListItem>.Fail("Failed to delete transaction for the unbought item.");

                var result = await DeleteTransactionAsync(userId, item.FinanceTransactionId.Id, token);
                item.RemoveFinanceTransaction();

                if (!result.IsSuccess)
                {
                    return Result<ShoppingListItem>.Fail("Failed to delete transaction for the unbought item.");
                }
            }

            var updated = await shoppingListRepository.UpdateItemAsync(item, token);
            return Result<ShoppingListItem>.Ok(updated);
        }, ct);
    }

    public async Task<Result> ReorderShoppingListItemsAsync(Guid userId, Guid listId, List<Guid> itemIds, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var list = await shoppingListRepository.GetAsync(listId, ct);
        if (list is null)
            return Result.NotFound("Shopping list not found.");

        if (list.TeamId.Id != teamId)
            return Result.Forbidden("Access denied.");

        var existingIds = list.Items.Select(i => i.Id.Id).ToHashSet();
        var providedIds = itemIds.ToHashSet();

        if (existingIds.Count != itemIds.Count || !existingIds.SetEquals(providedIds))
            return Result.Fail("Item IDs do not match the items in the list.", ErrorType.Validation);

        await shoppingListRepository.ReorderItemsAsync(listId, itemIds, ct);
        return Result.Ok();
    }

    public async Task<Result<ShoppingListItem>> UpdateShoppingListItemDetailsAsync(Guid userId, Guid listId, Guid itemId, string? name, long? price, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var list = await shoppingListRepository.GetAsync(listId, ct);
        if (list is null)
            return Result<ShoppingListItem>.NotFound("Shopping list not found.");

        if (list.TeamId.Id != teamId)
            return Result<ShoppingListItem>.Forbidden("Access denied.");

        var item = await shoppingListRepository.GetItemAsync(itemId, ct);
        if (item is null)
            return Result<ShoppingListItem>.NotFound("Shopping list item not found.");

        if (item.ListId.Id != listId)
            return Result<ShoppingListItem>.NotFound("Shopping list item not found.");

        return await unitOfWork.ExecuteAsync(async token =>
        {
            long delta;
            try
            {
                delta = item.UpdateDetails(name, price);
            }
            catch (Exception ex)
            {
                return Result<ShoppingListItem>.Fail(ex.Message, ErrorType.Validation);
            }

            if (delta != 0)
            {
                var balance = await balanceRepository.GetAsync(teamId, token);
                if (balance is null)
                    return Result<ShoppingListItem>.Conflict("Balance not found.");

                balance.Adjust(delta);
                await balanceRepository.UpdateAsync(balance, token);
            }

            var updated = await shoppingListRepository.UpdateItemAsync(item, token);
            return Result<ShoppingListItem>.Ok(updated);
        }, ct);
    }

    private async Task<List<(string Label, decimal Value)>> GetWeeklyChartDataAsync(
        Guid teamId,
        DateTime now,
        CancellationToken ct)
    {
        var result = new List<(string Label, decimal Value)>();

        var today = now.Date;
        var dayOfWeek = today.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)today.DayOfWeek - 1;
        var startOfWeek = today.AddDays(-dayOfWeek);

        string[] dayLabels = ["Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"];

        var from = DateTime.SpecifyKind(startOfWeek, DateTimeKind.Utc);
        var to = from.AddDays(7);

        var transactions = await transactionRepository.GetByTeamInPeriodAsync(teamId, from, to, ct);

        for (var i = 0; i < 7; i++)
        {
            var dayStart = from.AddDays(i);
            var dayEnd = dayStart.AddDays(1);

            result.Add((dayLabels[i], CalculateExpenses(transactions, dayStart, dayEnd)));
        }

        return result;
    }

    private async Task<List<(string Label, decimal Value)>> GetMonthlyChartDataAsync(
        Guid teamId,
        DateTime now,
        CultureInfo culture,
        CancellationToken ct)
    {
        var result = new List<(string Label, decimal Value)>();

        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endOfMonth = startOfMonth.AddMonths(1);
        var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

        var transactions = await transactionRepository.GetByTeamInPeriodAsync(teamId, startOfMonth, endOfMonth, ct);

        var weekStart = startOfMonth;
        while (weekStart < endOfMonth)
        {
            var weekEnd = weekStart.AddDays(7);
            if (weekEnd > endOfMonth)
                weekEnd = endOfMonth;

            var label = $"{weekStart.Day}–{Math.Min(weekEnd.AddDays(-1).Day, daysInMonth)} {weekStart.ToString("MMM", culture)}";
            result.Add((label, CalculateExpenses(transactions, weekStart, weekEnd)));

            weekStart = weekEnd;
        }

        return result;
    }

    private async Task<List<(string Label, decimal Value)>> GetYearlyChartDataAsync(
        Guid teamId,
        DateTime now,
        CultureInfo culture,
        CancellationToken ct)
    {
        var result = new List<(string Label, decimal Value)>();

        var startOfYear = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endOfYear = startOfYear.AddYears(1);

        var transactions = await transactionRepository.GetByTeamInPeriodAsync(teamId, startOfYear, endOfYear, ct);

        for (var month = 1; month <= 12; month++)
        {
            var monthStart = new DateTime(now.Year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1);

            var label = culture.DateTimeFormat.GetAbbreviatedMonthName(month);
            result.Add((label, CalculateExpenses(transactions, monthStart, monthEnd)));
        }

        return result;
    }

    private static decimal CalculateExpenses(
        IEnumerable<FinanceTransaction> transactions,
        DateTime from,
        DateTime to)
    {
        var expenses = transactions
            .Where(t => t.CreatedAt >= from && t.CreatedAt < to && t.Amount < 0)
            .Sum(t => Math.Abs(t.Amount));

        return Math.Round((decimal)expenses / 100, 2);
    }

    private static long CalculateBalanceDelta(
        bool wasFromBalance,
        long previousAmount,
        bool isFromBalance,
        long currentAmount)
    {
        var previousImpact = wasFromBalance ? previousAmount : 0L;
        var currentImpact = isFromBalance ? currentAmount : 0L;

        return currentImpact - previousImpact;
    }

    public async Task<Result<List<RecurringPayment>>> GetRecurringPaymentsAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var payments = await recurringPaymentRepository.GetByTeamAsync(teamId, ct);
        return Result<List<RecurringPayment>>.Ok(payments);
    }

    public async Task<Result<RecurringPayment>> CreateRecurringPaymentAsync(
        Guid userId, string title, long amount, int dayOfMonth, Guid? categoryId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        FinanceCategoryId? finCategoryId = null;
        if (categoryId.HasValue)
        {
            var category = await categoryRepository.GetAsync(categoryId.Value, ct);
            if (category is null || category.TeamId.Id != teamId)
                return Result<RecurringPayment>.NotFound("Category not found.");
            finCategoryId = category.Id;
        }

        var payment = RecurringPayment.Create(
            creatorId: new UserId(userId),
            teamId: new TeamId(teamId),
            title: title,
            amount: amount,
            dayOfMonth: dayOfMonth,
            categoryId: finCategoryId);

        var created = await recurringPaymentRepository.CreateAsync(payment, ct);
        return Result<RecurringPayment>.Ok(created);
    }

    public async Task<Result<RecurringPayment>> UpdateRecurringPaymentAsync(
        Guid userId, Guid paymentId, string? title, long? amount, int? dayOfMonth,
        Guid? categoryId, bool clearCategory, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var payment = await recurringPaymentRepository.GetAsync(paymentId, ct);
        if (payment is null)
            return Result<RecurringPayment>.NotFound("Recurring payment not found.");

        if (payment.TeamId.Id != teamId)
            return Result<RecurringPayment>.Forbidden("Access denied.");

        FinanceCategoryId? finCategoryId = null;
        if (!clearCategory && categoryId.HasValue)
        {
            var category = await categoryRepository.GetAsync(categoryId.Value, ct);
            if (category is null || category.TeamId.Id != teamId)
                return Result<RecurringPayment>.NotFound("Category not found.");
            finCategoryId = category.Id;
        }

        payment.Update(title, amount, dayOfMonth, finCategoryId, clearCategory);
        var updated = await recurringPaymentRepository.UpdateAsync(payment, ct);
        return Result<RecurringPayment>.Ok(updated);
    }

    public async Task<Result> DeleteRecurringPaymentAsync(Guid userId, Guid paymentId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var payment = await recurringPaymentRepository.GetAsync(paymentId, ct);
        if (payment is null)
            return Result.NotFound("Recurring payment not found.");

        if (payment.TeamId.Id != teamId)
            return Result.Forbidden("Access denied.");

        await recurringPaymentRepository.DeleteAsync(paymentId, ct);
        return Result.Ok();
    }

    public async Task<Result<List<PlannedPurchase>>> GetPlannedPurchasesAsync(
        Guid userId,
        DateOnly? from,
        DateOnly? to,
        PlannedPurchaseStatus? status,
        Guid? assigneeId,
        Guid? categoryId,
        CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var items = await plannedPurchaseRepository.GetByTeamAsync(
            teamId, from, to, status, assigneeId, categoryId, ct);
        return Result<List<PlannedPurchase>>.Ok(items);
    }

    public async Task<Result<PlannedPurchase>> CreatePlannedPurchaseAsync(
        Guid userId,
        string title,
        DateOnly date,
        long amount,
        Guid? assigneeId,
        Guid? categoryId,
        string? note,
        CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        UserId? assignee = null;
        if (assigneeId.HasValue)
        {
            if (!await IsTeamMemberAsync(teamId, assigneeId.Value, ct))
                return Result<PlannedPurchase>.Fail("Assignee is not a team member.", ErrorType.Validation);
            assignee = new UserId(assigneeId.Value);
        }

        FinanceCategoryId? finCategoryId = null;
        if (categoryId.HasValue)
        {
            var category = await categoryRepository.GetAsync(categoryId.Value, ct);
            if (category is null || category.TeamId.Id != teamId)
                return Result<PlannedPurchase>.NotFound("Category not found.");
            finCategoryId = category.Id;
        }

        PlannedPurchase purchase;
        try
        {
            purchase = PlannedPurchase.Create(
                ownerId: new UserId(userId),
                teamId: new TeamId(teamId),
                title: title,
                date: date,
                amount: amount,
                assigneeId: assignee,
                categoryId: finCategoryId,
                note: note);
        }
        catch (Exception ex)
        {
            return Result<PlannedPurchase>.Fail(ex.Message, ErrorType.Validation);
        }

        var created = await plannedPurchaseRepository.CreateAsync(purchase, ct);
        return Result<PlannedPurchase>.Ok(created);
    }

    public async Task<Result<PlannedPurchase>> UpdatePlannedPurchaseAsync(
        Guid userId,
        Guid purchaseId,
        string? title,
        DateOnly? date,
        long? amount,
        Guid? assigneeId,
        bool clearAssignee,
        Guid? categoryId,
        bool clearCategory,
        string? note,
        bool clearNote,
        PlannedPurchaseStatus? status,
        CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var purchase = await plannedPurchaseRepository.GetAsync(purchaseId, ct);
        if (purchase is null)
            return Result<PlannedPurchase>.NotFound("Planned purchase not found.");

        if (purchase.TeamId.Id != teamId)
            return Result<PlannedPurchase>.Forbidden("Access denied.");

        UserId? assignee = null;
        if (!clearAssignee && assigneeId.HasValue)
        {
            if (!await IsTeamMemberAsync(teamId, assigneeId.Value, ct))
                return Result<PlannedPurchase>.Fail("Assignee is not a team member.", ErrorType.Validation);
            assignee = new UserId(assigneeId.Value);
        }

        FinanceCategoryId? finCategoryId = null;
        if (!clearCategory && categoryId.HasValue)
        {
            var category = await categoryRepository.GetAsync(categoryId.Value, ct);
            if (category is null || category.TeamId.Id != teamId)
                return Result<PlannedPurchase>.NotFound("Category not found.");
            finCategoryId = category.Id;
        }

        try
        {
            purchase.Update(
                title: title,
                date: date,
                amount: amount,
                assigneeId: assignee,
                clearAssignee: clearAssignee,
                categoryId: finCategoryId,
                clearCategory: clearCategory,
                note: note,
                clearNote: clearNote,
                status: status);
        }
        catch (Exception ex)
        {
            return Result<PlannedPurchase>.Fail(ex.Message, ErrorType.Validation);
        }

        var updated = await plannedPurchaseRepository.UpdateAsync(purchase, ct);
        return Result<PlannedPurchase>.Ok(updated);
    }

    public async Task<Result> DeletePlannedPurchaseAsync(Guid userId, Guid purchaseId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var purchase = await plannedPurchaseRepository.GetAsync(purchaseId, ct);
        if (purchase is null)
            return Result.NotFound("Planned purchase not found.");

        if (purchase.TeamId.Id != teamId)
            return Result.Forbidden("Access denied.");

        await plannedPurchaseRepository.DeleteAsync(purchaseId, ct);
        return Result.Ok();
    }

    private async Task<bool> IsTeamMemberAsync(Guid teamId, Guid userId, CancellationToken ct)
    {
        var team = await teamRepository.GetAsync(teamId, ct);
        if (team is null) return false;
        return team.TeamMembers.Any(m => m.UserId.Id == userId);
    }
}
