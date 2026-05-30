using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Services;

public interface IAccountService
{
    Task<Result<List<Account>>> GetAsync(Guid userId, CancellationToken ct = default);

    Task<Result<Account>> CreateAsync(
        Guid userId, string name, string currencyCode, decimal balance, CancellationToken ct = default);

    Task<Result<Account>> UpdateAsync(
        Guid userId, Guid accountId,
        string? name, string? currencyCode, decimal? balance,
        CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid userId, Guid accountId, CancellationToken ct = default);

    /// <summary>Возвращает счета команды + общую сумму в рублях. Если у валюты нет курса —
    /// конвертация для этого счёта возвращается как null, и он не учитывается в TotalRub.</summary>
    Task<Result<AccountsTotal>> GetTotalAsync(Guid userId, CancellationToken ct = default);

    Task<Result<List<AccountTransaction>>> GetTransactionsAsync(
        Guid userId, Guid? accountId, CancellationToken ct = default);

    Task<Result<AccountTransaction>> CreateTransactionAsync(
        Guid userId, Guid accountId, Guid? categoryId,
        string title, decimal amount, DateTime? createdAt,
        CancellationToken ct = default);

    Task<Result<AccountTransaction>> UpdateTransactionAsync(
        Guid userId, Guid transactionId,
        Guid? categoryId, string? title, decimal? amount, DateTime? createdAt,
        CancellationToken ct = default);

    Task<Result> DeleteTransactionAsync(Guid userId, Guid transactionId, CancellationToken ct = default);
}

public sealed record AccountsTotal(
    decimal TotalRub,
    IReadOnlyList<AccountTotalItem> Items);

public sealed record AccountTotalItem(
    Account Account,
    decimal? ConvertedRub,
    Currency? Currency);
