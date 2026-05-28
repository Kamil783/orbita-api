using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class AccountExtensions
{
    public static AccountEntity ToEntity(this Account a) => new()
    {
        Id = a.Id.Id,
        CreatorId = a.CreatorId.Id,
        TeamId = a.TeamId.Id,
        Name = a.Name,
        CurrencyCode = a.CurrencyCode,
        Balance = a.Balance,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };

    public static Account ToDomain(this AccountEntity e) =>
        Account.Restore(
            new AccountId(e.Id),
            new UserId(e.CreatorId),
            new TeamId(e.TeamId),
            e.Name,
            e.CurrencyCode,
            e.Balance,
            e.CreatedAt,
            e.UpdatedAt);
}
