using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class AccountTransactionExtensions
{
    public static AccountTransactionEntity ToEntity(this AccountTransaction t) => new()
    {
        Id = t.Id.Id,
        AccountId = t.AccountId.Id,
        CreatorId = t.CreatorId.Id,
        TeamId = t.TeamId.Id,
        CategoryId = t.CategoryId?.Id,
        Title = t.Title,
        Amount = t.Amount,
        CreatedAt = t.CreatedAt
    };

    public static AccountTransaction ToDomain(this AccountTransactionEntity e) =>
        AccountTransaction.Restore(
            new AccountTransactionId(e.Id),
            new AccountId(e.AccountId),
            new UserId(e.CreatorId),
            new TeamId(e.TeamId),
            e.CategoryId.HasValue ? new FinanceCategoryId(e.CategoryId.Value) : null,
            e.Title,
            e.Amount,
            e.CreatedAt);
}
