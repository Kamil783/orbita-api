using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class FinanceCategoryExtensions
{
    public static FinanceCategoryEntity ToEntity(this FinanceCategory category)
    {
        return new FinanceCategoryEntity
        {
            Id = category.Id.Id,
            CreatorId = category.CreatorId.Id,
            Name = category.Name,
            Icon = category.Icon,
            Bg = category.Bg,
            Color = category.Color,
            WeeklyLimit = category.WeeklyLimit,
            MonthlyLimit = category.MonthlyLimit
        };
    }

    public static FinanceCategory ToDomain(this FinanceCategoryEntity entity)
    {
        return FinanceCategory.Restore(
            id: new FinanceCategoryId(entity.Id),
            creatorId: new UserId(entity.CreatorId),
            name: entity.Name,
            icon: entity.Icon,
            bg: entity.Bg,
            color: entity.Color,
            weeklyLimit: entity.WeeklyLimit,
            monthlyLimit: entity.MonthlyLimit
        );
    }
}
