using Microsoft.Extensions.Logging;
using Orbita.Application.Abstractions.Jobs;
using Orbita.Application.Abstractions.Repositories;

namespace Orbita.Infrastructure.Jobs;

public class MonthRolloverJob(
    IFinanceBalanceRepository balanceRepository,
    ILogger<MonthRolloverJob> logger) : IDailyJob
{
    public string Name => "MonthRollover";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var currentMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var balances = await balanceRepository.GetAllAsync(ct);

        foreach (var balance in balances)
        {
            var needsRollover = balance.LastMonthClosedAt is null
                || balance.LastMonthClosedAt.Value < currentMonth;

            if (!needsRollover)
                continue;

            logger.LogInformation(
                "Rolling over month for team {TeamId}: balance {Balance} -> previousMonthBalance",
                balance.TeamId.Id, balance.Balance);

            balance.CloseMonth();
            await balanceRepository.UpdateAsync(balance, ct);
        }
    }
}
