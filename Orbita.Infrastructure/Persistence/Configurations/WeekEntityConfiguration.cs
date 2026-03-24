using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class WeekEntityConfiguration : IEntityTypeConfiguration<WeekEntity>
{
    public void Configure(EntityTypeBuilder<WeekEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever();

        b.Property(x => x.CreatorId)
            .IsRequired();

        b.Property(x => x.StartDate)
            .IsRequired();

        b.Property(x => x.EndDate)
            .IsRequired();

        b.Property(x => x.IsArchived)
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .IsRequired();

        b.HasIndex(x => x.CreatorId);
        b.HasIndex(x => new { x.CreatorId, x.IsArchived });
    }
}
