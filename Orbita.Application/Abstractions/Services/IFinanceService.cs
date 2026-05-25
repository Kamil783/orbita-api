using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;

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
    Task<Result<SavingsGoal>> UpdateSavingsGoalDetailsAsync(Guid userId, Guid goalId, string? name, long? target, CancellationToken ct = default);
    Task<Result<SavingsGoal>> TopUpSavingsGoalAsync(Guid userId, Guid goalId, long amount, CancellationToken ct = default);
    Task<Result<SavingsGoal>> WithdrawFromSavingsGoalAsync(Guid userId, Guid goalId, long amount, CancellationToken ct = default); 
    Task<Result> DeleteSavingsGoalAsync(Guid userId, Guid goalId, CancellationToken ct = default);

    Task<Result<SpendingLimit>> GetSpendingLimitsAsync(Guid userId, CancellationToken ct = default);
    Task<Result<SpendingLimit>> UpdateSpendingLimitsAsync(Guid userId, long monthlyLimit, long weeklyLimit, CancellationToken ct = default);

    Task<Result<List<(string Label, decimal Value)>>> GetChartDataAsync(Guid userId, string period, CancellationToken ct = default);

    Task<Result<List<ShoppingList>>> GetShoppingListsAsync(Guid userId, CancellationToken ct = default);
    Task<Result<ShoppingList>> CreateShoppingListAsync(Guid userId, string name, bool fromBalance, CancellationToken ct = default);
    Task<Result<ShoppingList>> UpdateShoppingListAsync(Guid userId, Guid listId, string? name, bool? pinned, bool? isFromBalance, CancellationToken ct = default);
    Task<Result> DeleteShoppingListAsync(Guid userId, Guid listId, CancellationToken ct = default);
    Task<Result<ShoppingListItem>> AddShoppingListItemAsync(Guid userId, Guid listId, string name, long? price, CancellationToken ct = default);
    Task<Result> DeleteShoppingListItemAsync(Guid userId, Guid listId, Guid itemId, CancellationToken ct = default);
    Task<Result<ShoppingListItem>> UpdateShoppingListItemAsync(Guid userId, Guid listId, Guid itemId, bool bought, CancellationToken ct = default);
    Task<Result<ShoppingListItem>> UpdateShoppingListItemDetailsAsync(Guid userId, Guid listId, Guid itemId, string? name, long? price, CancellationToken ct = default);
    Task<Result> ReorderShoppingListItemsAsync(Guid userId, Guid listId, List<Guid> itemIds, CancellationToken ct = default);

    Task<Result<List<RecurringPayment>>> GetRecurringPaymentsAsync(Guid userId, CancellationToken ct = default);
    Task<Result<RecurringPayment>> CreateRecurringPaymentAsync(Guid userId, string title, long amount, int dayOfMonth, Guid? categoryId, CancellationToken ct = default);
    Task<Result<RecurringPayment>> UpdateRecurringPaymentAsync(Guid userId, Guid paymentId, string? title, long? amount, int? dayOfMonth, Guid? categoryId, bool clearCategory, CancellationToken ct = default);
    Task<Result> DeleteRecurringPaymentAsync(Guid userId, Guid paymentId, CancellationToken ct = default);

    Task<Result<List<PlannedPurchase>>> GetPlannedPurchasesAsync(
        Guid userId,
        DateOnly? from,
        DateOnly? to,
        PlannedPurchaseStatus? status,
        PlannedPurchaseDirection? direction,
        PlannedPurchaseAssigneeKind? assigneeKind,
        Guid? assigneeUserId,
        Guid? categoryId,
        CancellationToken ct = default);

    Task<Result<PlannedPurchase>> CreatePlannedPurchaseAsync(
        Guid userId,
        string title,
        DateOnly date,
        PlannedPurchaseDirection direction,
        long amount,
        long? actualAmount,
        PlannedPurchaseAssigneeKind? assigneeKind,
        Guid? assigneeUserId,
        Guid? categoryId,
        string? note,
        CancellationToken ct = default);

    Task<Result<PlannedPurchase>> UpdatePlannedPurchaseAsync(
        Guid userId,
        Guid purchaseId,
        string? title,
        DateOnly? date,
        PlannedPurchaseDirection? direction,
        long? amount,
        long? actualAmount,
        PlannedPurchaseAssigneeKind? assigneeKind,
        Guid? assigneeUserId,
        Guid? categoryId,
        string? note,
        PlannedPurchaseStatus? status,
        CancellationToken ct = default);

    Task<Result> DeletePlannedPurchaseAsync(Guid userId, Guid purchaseId, CancellationToken ct = default);
}
