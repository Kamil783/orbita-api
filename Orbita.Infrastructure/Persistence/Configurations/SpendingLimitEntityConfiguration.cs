using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class SpendingLimitEntityConfiguration : IEntityTypeConfiguration<SpendingLimitEntity>
{
    public void Configure(EntityTypeBuilder<SpendingLimitEntity> b)
    {
        b.HasKey(x => x.UserId);

        b.Property(x => x.UserId)
            .ValueGeneratedNever();

        b.Property(x => x.MonthlyLimit)
            .IsRequired();

        b.Property(x => x.WeeklyLimit)
            .IsRequired();
    }
}
