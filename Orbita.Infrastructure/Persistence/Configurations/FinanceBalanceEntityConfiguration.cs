using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class FinanceBalanceEntityConfiguration : IEntityTypeConfiguration<FinanceBalanceEntity>
{
    public void Configure(EntityTypeBuilder<FinanceBalanceEntity> b)
    {
        b.HasKey(x => x.UserId);

        b.Property(x => x.UserId)
            .ValueGeneratedNever();

        b.Property(x => x.Balance)
            .IsRequired();

        b.Property(x => x.UpdatedAt)
            .IsRequired();
    }
}
