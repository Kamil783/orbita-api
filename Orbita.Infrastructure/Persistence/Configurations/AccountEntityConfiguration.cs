using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class AccountEntityConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.CreatorId).IsRequired();
        b.Property(x => x.TeamId).IsRequired();

        b.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.CurrencyCode)
            .HasMaxLength(10)
            .IsRequired();

        b.Property(x => x.Balance)
            .HasColumnType("numeric(28,8)")
            .IsRequired();

        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt).IsRequired();

        b.HasOne(x => x.Currency)
            .WithMany(c => c.Accounts)
            .HasForeignKey(x => x.CurrencyCode)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.TeamId);
        b.HasIndex(x => new { x.TeamId, x.CurrencyCode });
    }
}
