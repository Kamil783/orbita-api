using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class FinanceTransactionEntityConfiguration : IEntityTypeConfiguration<FinanceTransactionEntity>
{
    public void Configure(EntityTypeBuilder<FinanceTransactionEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever();

        b.Property(x => x.CreatorId)
            .IsRequired();

        b.Property(x => x.CategoryId)
            .IsRequired(false);

        b.Property(x => x.Title)
            .HasMaxLength(500)
            .IsRequired();

        b.Property(x => x.Amount)
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .IsRequired();

        b.Property(x => x.IsFromBalance)
            .IsRequired();

        b.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.CreatorId);
        b.HasIndex(x => x.CategoryId);
        b.HasIndex(x => new { x.CreatorId, x.CreatedAt });
    }
}
