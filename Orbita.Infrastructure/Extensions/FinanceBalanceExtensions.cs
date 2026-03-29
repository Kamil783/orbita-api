using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class FinanceBalanceExtensions
{
    public static FinanceBalanceEntity ToEntity(this FinanceBalance balance)
    {
        return new FinanceBalanceEntity
        {
            TeamId = balance.TeamId.Id,
            Balance = balance.Balance,
            PreviousMonthBalance = balance.PreviousMonthBalance,
            LastMonthClosedAt = balance.LastMonthClosedAt,
            UpdatedAt = balance.UpdatedAt
        };
    }

    public static FinanceBalance ToDomain(this FinanceBalanceEntity entity)
    {
        return FinanceBalance.Restore(
            teamId: new TeamId(entity.TeamId),
            balance: entity.Balance,
            previousMonthBalance: entity.PreviousMonthBalance,
            lastMonthClosedAt: entity.LastMonthClosedAt,
            updatedAt: entity.UpdatedAt
        );
    }
}
