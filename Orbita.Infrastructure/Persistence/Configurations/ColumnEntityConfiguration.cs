using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Persistence.Configurations.Converters;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class ColumnEntityConfiguration : IEntityTypeConfiguration<ColumnEntity>
{
    public void Configure(EntityTypeBuilder<ColumnEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasConversion(IdConverters.GuidId(
                id => id.Id,
                value => new ColumnId(value)))
            .ValueGeneratedNever();

        b.Property(x => x.Title)
            .HasMaxLength(120)
            .IsRequired();

        b.Property(x => x.TotalCount).IsRequired();

        b.Property(x => x.HeaderActionIcon)
            .HasMaxLength(64)
            .IsRequired();

        b.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.Muted).IsRequired();

        b.HasMany(x => x.TodoItems)
            .WithOne(x => x.Column)
            .HasForeignKey(x => x.ColumnId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.Status);
    }
}