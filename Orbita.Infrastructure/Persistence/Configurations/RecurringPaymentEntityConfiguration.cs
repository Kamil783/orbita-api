using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class RecurringPaymentEntityConfiguration : IEntityTypeConfiguration<RecurringPaymentEntity>
{
    public void Configure(EntityTypeBuilder<RecurringPaymentEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever();

        b.Property(x => x.CreatorId)
            .IsRequired();

        b.Property(x => x.TeamId)
            .IsRequired();

        b.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Amount)
            .IsRequired();

        b.Property(x => x.DayOfMonth)
            .IsRequired();

        b.Property(x => x.CategoryId)
            .IsRequired(false);

        b.Property(x => x.CreatedAt)
            .IsRequired();

        b.Property(x => x.UpdatedAt)
            .IsRequired();

        b.HasIndex(x => x.TeamId);
    }
}
