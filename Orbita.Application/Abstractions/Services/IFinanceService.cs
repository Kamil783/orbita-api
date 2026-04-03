using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Services;

public interface IFinanceService
{
    Task<Result<FinanceBalance>> GetBalanceAsync(Guid userId, CancellationToken ct = default);
    Task<Result<FinanceBalance>> GetPreviousMonthBalanceAsync(Guid userId, CancellationToken ct = default);
    Task<Result<FinanceBalance>> AdjustBalanceAsync(Guid userId, long amount, CancellationToken ct = default);

    Task<Result<List<FinanceCategory>>> GetCategoriesAsync(Guid userId, CancellationToken ct = default);
    Task<Result<FinanceCategory>> CreateCategoryAsync(Guid userId, string name, string icon, string bg, string color, long? weeklyLimit, long? monthlyLimit, CancellationToken ct = default);
    Task<Result<FinanceCategory>> UpdateCategoryAsync(Guid userId, Guid categoryId, string? name, string? icon, string? bg, string? color, long? weeklyLimit, long? monthlyLimit, CancellationToken ct = default);

    Task<Result<List<FinanceTransaction>>> GetTransactionsAsync(Guid userId, CancellationToken ct = default);
    Task<Result<FinanceTransaction>> CreateTransactionAsync(Guid userId, Guid? categoryId, string title, long amount, bool fromBalance, DateTime? createdAt, CancellationToken ct = default);
    Task<Result<FinanceTransaction>> UpdateTransactionAsync(Guid userId, Guid transactionId, Guid? categoryId, string? title, long? amount, bool? fromBalance, DateTime? createdAt, CancellationToken ct = default);
    Task<Result> DeleteTransactionAsync(Guid userId, Guid transactionId, CancellationToken ct = default);

    Task<Result<List<SavingsGoal>>> GetSavingsGoalsAsync(Guid userId, CancellationToken ct = default);
    Task<Result<SavingsGoal>> CreateSavingsGoalAsync(Guid userId, string name, long target, CancellationToken ct = default);
    Task<Result<SavingsGoal>> TopUpSavingsGoalAsync(Guid userId, Guid goalId, long amount, CancellationToken ct = default);
    Task<Result<SavingsGoal>> WithdrawFromSavingsGoalAsync(Guid userId, Guid goalId, long amount, CancellationToken ct = default); 
    Task<Result> DeleteSavingsGoalAsync(Guid userId, Guid goalId, CancellationToken ct = default);

    Task<Result<SpendingLimit>> GetSpendingLimitsAsync(Guid userId, CancellationToken ct = default);
    Task<Result<SpendingLimit>> UpdateSpendingLimitsAsync(Guid userId, long monthlyLimit, long weeklyLimit, CancellationToken ct = default);

    Task<Result<List<(string Label, decimal Value)>>> GetChartDataAsync(Guid userId, string period, CancellationToken ct = default);

    Task<Result<List<ShoppingList>>> GetShoppingListsAsync(Guid userId, CancellationToken ct = default);
    Task<Result<ShoppingList>> CreateShoppingListAsync(Guid userId, string name, CancellationToken ct = default);
    Task<Result> DeleteShoppingListAsync(Guid userId, Guid listId, CancellationToken ct = default);
    Task<Result<ShoppingListItem>> AddShoppingListItemAsync(Guid userId, Guid listId, string name, long? price, CancellationToken ct = default);
    Task<Result> DeleteShoppingListItemAsync(Guid userId, Guid listId, Guid itemId, CancellationToken ct = default);
    Task<Result<ShoppingListItem>> UpdateShoppingListItemAsync(Guid userId, Guid listId, Guid itemId, bool bought, CancellationToken ct = default);
}
