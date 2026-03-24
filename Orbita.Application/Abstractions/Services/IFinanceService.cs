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

    Task<Result<List<FinanceTransaction>>> GetTransactionsAsync(Guid userId, CancellationToken ct = default);
    Task<Result<FinanceTransaction>> CreateTransactionAsync(Guid userId, Guid categoryId, string title, long amount, CancellationToken ct = default);
    Task<Result> DeleteTransactionAsync(Guid userId, Guid transactionId, CancellationToken ct = default);

    Task<Result<List<SavingsGoal>>> GetSavingsGoalsAsync(Guid userId, CancellationToken ct = default);
    Task<Result<SavingsGoal>> CreateSavingsGoalAsync(Guid userId, string name, long target, CancellationToken ct = default);

    Task<Result<SpendingLimit>> GetSpendingLimitsAsync(Guid userId, CancellationToken ct = default);
    Task<Result<SpendingLimit>> UpdateSpendingLimitsAsync(Guid userId, long monthlyLimit, long weeklyLimit, CancellationToken ct = default);

    Task<Result<List<(string Label, decimal Value)>>> GetChartDataAsync(Guid userId, string period, CancellationToken ct = default);
}
