using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;

namespace Orbita.Application.Services;

public class AccountService(
    IAccountRepository accountRepository,
    ICurrencyRepository currencyRepository,
    ITeamProvider teamProvider) : IAccountService
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
}
