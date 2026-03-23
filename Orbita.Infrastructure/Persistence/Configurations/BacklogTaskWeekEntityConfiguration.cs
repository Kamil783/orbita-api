using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities.Mapping;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class BacklogTaskWeekEntityConfiguration : IEntityTypeConfiguration<BacklogTaskWeekEntity>
{
    public void Configure(EntityTypeBuilder<BacklogTaskWeekEntity> b)
    {
        b.HasKey(x => new { x.BacklogTaskId, x.WeekId });

        b.HasOne(x => x.BacklogTask)
            .WithMany(t => t.BacklogTaskWeeks)
            .HasForeignKey(x => x.BacklogTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Week)
            .WithMany(w => w.BacklogTaskWeeks)
            .HasForeignKey(x => x.WeekId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.WeekId);
    }
}
