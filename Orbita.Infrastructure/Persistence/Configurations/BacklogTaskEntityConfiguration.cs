using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class BacklogTaskEntityConfiguration : IEntityTypeConfiguration<BacklogTaskEntity>
{
    public void Configure(EntityTypeBuilder<BacklogTaskEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever();

        b.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Priority)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.Description)
            .HasMaxLength(4000)
            .IsRequired();

        b.Property(x => x.CreatorId)
            .IsRequired();


        b.Property(x => x.CreatedAt)
            .IsRequired();

        b.Property(x => x.InWeek).IsRequired();
        b.Property(x => x.IsCompleted).IsRequired();

        b.Property(x => x.DueTime);

        b.Property(x => x.EstimateMinutes);

        b.HasIndex(x => x.CreatorId);
        b.HasIndex(x => new { x.IsCompleted, x.InWeek });
    }
}
