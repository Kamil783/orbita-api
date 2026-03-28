using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class FinanceTransactionExtensions
{
    public static FinanceTransactionEntity ToEntity(this FinanceTransaction transaction)
    {
        return new FinanceTransactionEntity
        {
            Id = transaction.Id.Id,
            CreatorId = transaction.CreatorId.Id,
            CategoryId = transaction.CategoryId?.Id,
            Title = transaction.Title,
            Amount = transaction.Amount,
            CreatedAt = transaction.CreatedAt,
            IsFromBalance = transaction.IsFromBalance
        };
    }

    public static FinanceTransaction ToDomain(this FinanceTransactionEntity entity)
    {
        return FinanceTransaction.Restore(
            id: new FinanceTransactionId(entity.Id),
            creatorId: new UserId(entity.CreatorId),
            categoryId: entity.CategoryId.HasValue ? new FinanceCategoryId(entity.CategoryId.Value) : null,
            title: entity.Title,
            amount: entity.Amount,
            createdAt: entity.CreatedAt,
            isFromBalance: entity.IsFromBalance
        );
    }
}
