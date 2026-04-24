using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class RecurringPaymentExtensions
{
    public static RecurringPaymentEntity ToEntity(this RecurringPayment payment)
    {
        return new RecurringPaymentEntity
        {
            Id = payment.Id.Id,
            CreatorId = payment.CreatorId.Id,
            TeamId = payment.TeamId.Id,
            Title = payment.Title,
            Amount = payment.Amount,
            DayOfMonth = payment.DayOfMonth,
            CategoryId = payment.CategoryId?.Id,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt
        };
    }

    public static RecurringPayment ToDomain(this RecurringPaymentEntity entity)
    {
        return RecurringPayment.Restore(
            id: new RecurringPaymentId(entity.Id),
            creatorId: new UserId(entity.CreatorId),
            teamId: new TeamId(entity.TeamId),
            title: entity.Title,
            amount: entity.Amount,
            dayOfMonth: entity.DayOfMonth,
            categoryId: entity.CategoryId.HasValue ? new FinanceCategoryId(entity.CategoryId.Value) : null,
            createdAt: entity.CreatedAt,
            updatedAt: entity.UpdatedAt
        );
    }
}
