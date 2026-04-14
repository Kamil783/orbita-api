using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class TimeEntryEntityConfiguration : IEntityTypeConfiguration<TimeEntryEntity>
{
    public void Configure(EntityTypeBuilder<TimeEntryEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever();

        b.Property(x => x.BacklogTaskId)
            .IsRequired();

        b.Property(x => x.UserId)
            .IsRequired();

        b.Property(x => x.Minutes)
            .IsRequired();

        b.Property(x => x.Description)
            .HasMaxLength(500);

        b.Property(x => x.CreatedAt)
            .IsRequired();

        b.HasOne(x => x.BacklogTask)
            .WithMany(x => x.TimeEntries)
            .HasForeignKey(x => x.BacklogTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.BacklogTaskId);
    }
}
