using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class RecurringTaskEntityConfiguration : IEntityTypeConfiguration<RecurringTaskEntity>
{
    public void Configure(EntityTypeBuilder<RecurringTaskEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.CreatorId).IsRequired();
        b.Property(x => x.TeamId).IsRequired();

        b.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Description)
            .HasMaxLength(2000)
            .IsRequired(false);

        b.Property(x => x.DayOfMonth).IsRequired();
        b.Property(x => x.IsCompleted).IsRequired().HasDefaultValue(false);
        b.Property(x => x.LastResetAt).IsRequired(false);
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt).IsRequired();

        b.HasIndex(x => x.TeamId);
    }
}
