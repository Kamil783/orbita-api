using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class CurrencyEntityConfiguration : IEntityTypeConfiguration<CurrencyEntity>
{
    public void Configure(EntityTypeBuilder<CurrencyEntity> b)
    {
        b.HasKey(x => x.Code);

        b.Property(x => x.Code)
            .HasMaxLength(10)
            .IsRequired();

        b.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.NumCode).IsRequired(false);

        b.Property(x => x.Kind)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(Orbita.Domain.Enums.CurrencyKind.Fiat);

        b.Property(x => x.RateToRub)
            .HasColumnType("numeric(28,8)")
            .IsRequired(false);

        b.Property(x => x.Nominal)
            .IsRequired()
            .HasDefaultValue(1);

        b.Property(x => x.RateFetchedAt).IsRequired(false);

        b.HasIndex(x => x.Kind);
    }
}
