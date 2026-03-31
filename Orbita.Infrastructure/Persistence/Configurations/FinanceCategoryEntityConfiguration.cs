using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class FinanceCategoryEntityConfiguration : IEntityTypeConfiguration<FinanceCategoryEntity>
{
    public void Configure(EntityTypeBuilder<FinanceCategoryEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever();

        b.Property(x => x.CreatorId)
            .IsRequired();

        b.Property(x => x.TeamId)
            .IsRequired();

        b.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Icon)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.Bg)
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.Color)
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.WeeklyLimit)
            .IsRequired(false);

        b.Property(x => x.MonthlyLimit)
            .IsRequired(false);

        b.HasIndex(x => x.TeamId);
    }
}
