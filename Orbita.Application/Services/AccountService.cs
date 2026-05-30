using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;

namespace Orbita.Application.Services;

public class AccountService(
    IAccountRepository accountRepository,
    IAccountTransactionRepository accountTransactionRepository,
    ICurrencyRepository currencyRepository,
    IFinanceCategoryRepository categoryRepository,
    ITeamProvider teamProvider,
    Orbita.Application.Abstractions.IUnitOfWork unitOfWork) : IAccountService
{
    public async Task<Result<List<Account>>> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var accounts = await accountRepository.GetByTeamAsync(teamId, ct);
        return Result<List<Account>>.Ok(accounts);
    }

    public async Task<Result<Account>> CreateAsync(
        Guid userId, string name, string currencyCode, decimal balance, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var currency = await currencyRepository.GetAsync(currencyCode, ct);
        if (currency is null)
            return Result<Account>.NotFound($"Currency '{currencyCode}' not found.");

        Account account;
        try
        {
            account = Account.Create(
                creatorId: new UserId(userId),
                teamId: new TeamId(teamId),
                name: name,
                currencyCode: currency.Code,
                balance: balance);
        }
        catch (Exception ex)
        {
            return Result<Account>.Fail(ex.Message, ErrorType.Validation);
        }

        var created = await accountRepository.CreateAsync(account, ct);
        return Result<Account>.Ok(created);
    }

    public async Task<Result<Account>> UpdateAsync(
        Guid userId, Guid accountId,
        string? name, string? currencyCode, decimal? balance,
        CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var account = await accountRepository.GetAsync(accountId, ct);
        if (account is null)
            return Result<Account>.NotFound("Account not found.");

        if (account.TeamId.Id != teamId)
            return Result<Account>.Forbidden("Access denied.");

        if (currencyCode is not null)
        {
            var currency = await currencyRepository.GetAsync(currencyCode, ct);
            if (currency is null)
                return Result<Account>.NotFound($"Currency '{currencyCode}' not found.");
            currencyCode = currency.Code;
        }

        try
        {
            account.Update(name, currencyCode, balance);
        }
        catch (Exception ex)
        {
            return Result<Account>.Fail(ex.Message, ErrorType.Validation);
        }

        var updated = await accountRepository.UpdateAsync(account, ct);
        return Result<Account>.Ok(updated);
    }

    public async Task<Result> DeleteAsync(Guid userId, Guid accountId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var account = await accountRepository.GetAsync(accountId, ct);
        if (account is null)
            return Result.NotFound("Account not found.");

        if (account.TeamId.Id != teamId)
            return Result.Forbidden("Access denied.");

        await accountRepository.DeleteAsync(accountId, ct);
        return Result.Ok();
    }

    public async Task<Result<AccountsTotal>> GetTotalAsync(Guid userId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);
        var accounts = await accountRepository.GetByTeamAsync(teamId, ct);

        var currencies = (await currencyRepository.GetAllAsync(ct))
            .ToDictionary(c => c.Code, c => c, StringComparer.OrdinalIgnoreCase);

        var items = new List<AccountTotalItem>(accounts.Count);
        decimal total = 0m;

        foreach (var account in accounts)
        {
            currencies.TryGetValue(account.CurrencyCode, out var currency);
            var converted = currency?.ConvertToRub(account.Balance);

            if (converted.HasValue)
                total += converted.Value;

            items.Add(new AccountTotalItem(account, converted, currency));
        }

        return Result<AccountsTotal>.Ok(new AccountsTotal(total, items));
    }

    public async Task<Result<List<AccountTransaction>>> GetTransactionsAsync(
        Guid userId, Guid? accountId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        if (accountId.HasValue)
        {
            var account = await accountRepository.GetAsync(accountId.Value, ct);
            if (account is null)
                return Result<List<AccountTransaction>>.NotFound("Account not found.");
            if (account.TeamId.Id != teamId)
                return Result<List<AccountTransaction>>.Forbidden("Access denied.");

            var byAccount = await accountTransactionRepository.GetByAccountAsync(accountId.Value, ct);
            return Result<List<AccountTransaction>>.Ok(byAccount);
        }

        var byTeam = await accountTransactionRepository.GetByTeamAsync(teamId, ct);
        return Result<List<AccountTransaction>>.Ok(byTeam);
    }

    public async Task<Result<AccountTransaction>> CreateTransactionAsync(
        Guid userId, Guid accountId, Guid? categoryId,
        string title, decimal amount, DateTime? createdAt,
        CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var account = await accountRepository.GetAsync(accountId, ct);
        if (account is null)
            return Result<AccountTransaction>.NotFound("Account not found.");
        if (account.TeamId.Id != teamId)
            return Result<AccountTransaction>.Forbidden("Access denied.");

        FinanceCategoryId? finCategoryId = null;
        if (categoryId.HasValue)
        {
            var category = await categoryRepository.GetAsync(categoryId.Value, ct);
            if (category is null || category.TeamId.Id != teamId)
                return Result<AccountTransaction>.NotFound("Category not found.");
            finCategoryId = category.Id;
        }

        var utc = createdAt.HasValue
            ? DateTime.SpecifyKind(createdAt.Value, DateTimeKind.Utc)
            : DateTime.UtcNow;

        AccountTransaction transaction;
        try
        {
            transaction = AccountTransaction.Create(
                accountId: account.Id,
                creatorId: new UserId(userId),
                teamId: new TeamId(teamId),
                categoryId: finCategoryId,
                title: title,
                amount: amount,
                createdAt: utc);
        }
        catch (Exception ex)
        {
            return Result<AccountTransaction>.Fail(ex.Message, ErrorType.Validation);
        }

        return await unitOfWork.ExecuteAsync(async token =>
        {
            var created = await accountTransactionRepository.CreateAsync(transaction, token);

            account.Update(name: null, currencyCode: null, balance: account.Balance + amount);
            await accountRepository.UpdateAsync(account, token);

            return Result<AccountTransaction>.Ok(created);
        }, ct);
    }

    public async Task<Result<AccountTransaction>> UpdateTransactionAsync(
        Guid userId, Guid transactionId,
        Guid? categoryId, string? title, decimal? amount, DateTime? createdAt,
        CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var transaction = await accountTransactionRepository.GetAsync(transactionId, ct);
        if (transaction is null)
            return Result<AccountTransaction>.NotFound("Account transaction not found.");
        if (transaction.TeamId.Id != teamId)
            return Result<AccountTransaction>.Forbidden("Access denied.");

        var account = await accountRepository.GetAsync(transaction.AccountId.Id, ct);
        if (account is null)
            return Result<AccountTransaction>.NotFound("Account not found.");

        FinanceCategoryId? finCategoryId = null;
        if (categoryId.HasValue)
        {
            var category = await categoryRepository.GetAsync(categoryId.Value, ct);
            if (category is null || category.TeamId.Id != teamId)
                return Result<AccountTransaction>.NotFound("Category not found.");
            finCategoryId = category.Id;
        }

        var oldAmount = transaction.Amount;

        try
        {
            // categoryId → PUT-семантика (значение всегда применяется, null = снять).
            transaction.SetCategoryId(finCategoryId);

            if (title is not null)
                transaction.SetTitle(title);

            if (amount.HasValue)
                transaction.SetAmount(amount.Value);

            if (createdAt.HasValue)
                transaction.SetCreatedAt(createdAt.Value);
        }
        catch (Exception ex)
        {
            return Result<AccountTransaction>.Fail(ex.Message, ErrorType.Validation);
        }

        return await unitOfWork.ExecuteAsync(async token =>
        {
            var updated = await accountTransactionRepository.UpdateAsync(transaction, token);

            var delta = transaction.Amount - oldAmount;
            if (delta != 0m)
            {
                account.Update(name: null, currencyCode: null, balance: account.Balance + delta);
                await accountRepository.UpdateAsync(account, token);
            }

            return Result<AccountTransaction>.Ok(updated);
        }, ct);
    }

    public async Task<Result> DeleteTransactionAsync(Guid userId, Guid transactionId, CancellationToken ct = default)
    {
        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var transaction = await accountTransactionRepository.GetAsync(transactionId, ct);
        if (transaction is null)
            return Result.NotFound("Account transaction not found.");
        if (transaction.TeamId.Id != teamId)
            return Result.Forbidden("Access denied.");

        var account = await accountRepository.GetAsync(transaction.AccountId.Id, ct);
        // Если счёт каким-то образом удалён до транзакции — просто удалим саму операцию.

        return (await unitOfWork.ExecuteAsync<bool>(async token =>
        {
            await accountTransactionRepository.DeleteAsync(transactionId, token);

            if (account is not null)
            {
                account.Update(name: null, currencyCode: null, balance: account.Balance - transaction.Amount);
                await accountRepository.UpdateAsync(account, token);
            }

            return Result<bool>.Ok(true);
        }, ct)).IsSuccess ? Result.Ok() : Result.Fail("Delete failed.");
    }
}
