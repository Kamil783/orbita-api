using System.Globalization;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;

namespace Orbita.Application.Services;

public class FinanceService(
    IFinanceBalanceRepository balanceRepository,
    IFinanceCategoryRepository categoryRepository,
    IFinanceTransactionRepository transactionRepository,
    ISavingsGoalRepository savingsGoalRepository,
    ISpendingLimitRepository spendingLimitRepository,
    IShoppingListRepository shoppingListRepository,
    ITeamProvider teamProvider) : IFinanceService
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

    public async Task<Result<List<FinanceTransaction>>> GetTransactionsAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var transactions = await transactionRepository.GetByTeamAsync(teamId, ct);
        return Result<List<FinanceTransaction>>.Ok(transactions);
    }

    public async Task<Result<FinanceTransaction>> CreateTransactionAsync(
        Guid userId, Guid? categoryId, string title, long amount, bool fromBalance, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

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
            isFromBalance: fromBalance);

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
        }

        return Result<FinanceTransaction>.Ok(created);
    }

    public async Task<Result<FinanceTransaction>> UpdateTransactionAsync(
        Guid userId, Guid transactionId, Guid? categoryId, string? title, long? amount, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var transaction = await transactionRepository.GetAsync(transactionId, ct);
        if (transaction is null)
            return Result<FinanceTransaction>.NotFound("Transaction not found.");

        if (transaction.TeamId.Id != teamId)
            return Result<FinanceTransaction>.Forbidden("Access denied.");

        var oldAmount = transaction.Amount;

        if (categoryId.HasValue)
        {
            var category = await categoryRepository.GetAsync(categoryId.Value, ct);
            if (category is null)
                return Result<FinanceTransaction>.NotFound("Category not found.");

            transaction.SetCategoryId(new FinanceCategoryId(categoryId.Value));
        }

        if (title is not null)
            transaction.SetTitle(title);

        if (amount.HasValue)
        {
            transaction.SetAmount(amount.Value);

            if (transaction.IsFromBalance)
            {
                var balance = await balanceRepository.GetAsync(teamId, ct);
                if (balance is not null)
                {
                    balance.Adjust(-oldAmount + amount.Value);
                    await balanceRepository.UpdateAsync(balance, ct);
                }
            }
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

    public async Task<Result<SavingsGoal>> TopUpSavingsGoalAsync(Guid userId, Guid goalId, long amount, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var goal = await savingsGoalRepository.GetAsync(goalId, ct);
        if (goal is null)
            return Result<SavingsGoal>.NotFound("Savings goal not found.");

        if (goal.TeamId.Id != teamId)
            return Result<SavingsGoal>.Forbidden("Access denied.");

        goal.AddFunds(amount);
        var updated = await savingsGoalRepository.UpdateAsync(goal, ct);
        return Result<SavingsGoal>.Ok(updated);
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

        if (period == "weekly")
        {
            var result = new List<(string Label, decimal Value)>();
            var today = now.Date;
            var dayOfWeek = today.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)today.DayOfWeek - 1;
            var startOfWeek = today.AddDays(-dayOfWeek);

            string[] dayLabels = ["Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"];

            var from = DateTime.SpecifyKind(startOfWeek, DateTimeKind.Utc);
            var to = DateTime.SpecifyKind(startOfWeek.AddDays(7), DateTimeKind.Utc);
            var transactions = await transactionRepository.GetByTeamInPeriodAsync(teamId, from, to, ct);

            for (var i = 0; i < 7; i++)
            {
                var dayStart = startOfWeek.AddDays(i);
                var dayEnd = dayStart.AddDays(1);

                var dayExpenses = transactions
                    .Where(t => t.CreatedAt.Date == dayStart && t.Amount < 0)
                    .Sum(t => Math.Abs(t.Amount));

                result.Add((dayLabels[i], Math.Round((decimal)dayExpenses / 100, 2)));
            }

            return Result<List<(string Label, decimal Value)>>.Ok(result);
        }

        if (period == "monthly")
        {
            var result = new List<(string Label, decimal Value)>();
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var endOfMonth = startOfMonth.AddMonths(1);

            var transactions = await transactionRepository.GetByTeamInPeriodAsync(teamId, startOfMonth, endOfMonth, ct);

            var weekStart = startOfMonth;
            var weekNumber = 1;
            while (weekStart < endOfMonth)
            {
                var weekEnd = weekStart.AddDays(7);
                if (weekEnd > endOfMonth)
                    weekEnd = endOfMonth;

                var weekExpenses = transactions
                    .Where(t => t.CreatedAt >= weekStart && t.CreatedAt < weekEnd && t.Amount < 0)
                    .Sum(t => Math.Abs(t.Amount));

                var label = $"{weekStart.Day}–{Math.Min(weekEnd.AddDays(-1).Day, daysInMonth)} {weekStart.ToString("MMM", culture)}";
                result.Add((label, Math.Round((decimal)weekExpenses / 100, 2)));

                weekStart = weekEnd;
                weekNumber++;
            }

            return Result<List<(string Label, decimal Value)>>.Ok(result);
        }

        return Result<List<(string Label, decimal Value)>>.Fail("Invalid period. Use 'weekly' or 'monthly'.");
    }

    public async Task<Result<List<ShoppingList>>> GetShoppingListsAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var lists = await shoppingListRepository.GetByTeamAsync(teamId, ct);
        return Result<List<ShoppingList>>.Ok(lists);
    }

    public async Task<Result<ShoppingList>> CreateShoppingListAsync(Guid userId, string name, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var list = ShoppingList.Create(
            creatorId: new UserId(userId),
            teamId: new TeamId(teamId),
            name: name);

        var created = await shoppingListRepository.CreateAsync(list, ct);
        return Result<ShoppingList>.Ok(created);
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

        var item = ShoppingListItem.Create(
            listId: new ShoppingListId(listId),
            name: name,
            price: price);

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

        item.SetBought(bought);
        var updated = await shoppingListRepository.UpdateItemAsync(item, ct);
        return Result<ShoppingListItem>.Ok(updated);
    }
}
