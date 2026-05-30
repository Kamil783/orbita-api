using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class AccountTransactionEntityConfiguration : IEntityTypeConfiguration<AccountTransactionEntity>
{
    public void Configure(EntityTypeBuilder<AccountTransactionEntity> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.AccountId).IsRequired();
        b.Property(x => x.CreatorId).IsRequired();
        b.Property(x => x.TeamId).IsRequired();
        b.Property(x => x.CategoryId).IsRequired(false);

        b.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Amount)
            .HasColumnType("numeric(28,8)")
            .IsRequired();

        b.Property(x => x.CreatedAt).IsRequired();

        b.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.AccountId);
        b.HasIndex(x => new { x.TeamId, x.CreatedAt });
        b.HasIndex(x => new { x.AccountId, x.CreatedAt });
    }
}
