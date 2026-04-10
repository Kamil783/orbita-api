using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class ShoppingListItemEntityConfiguration : IEntityTypeConfiguration<ShoppingListItemEntity>
{
    public void Configure(EntityTypeBuilder<ShoppingListItemEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever();

        b.Property(x => x.ListId)
            .IsRequired();

        b.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Price);

        b.Property(x => x.Bought)
            .IsRequired();

        b.Property(x => x.Order)
            .IsRequired();

        b.HasIndex(x => new { x.ListId, x.Order });
    }
}
