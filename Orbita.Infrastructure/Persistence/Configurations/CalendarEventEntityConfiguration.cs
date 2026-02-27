using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Persistence.Configurations.Converters;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class CalendarEventEntityConfiguration : IEntityTypeConfiguration<CalendarEventEntity>
{
    public void Configure(EntityTypeBuilder<CalendarEventEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasConversion(IdConverters.GuidId(
                id => id.Id,
                value => new CalendarEventId(value)))
            .ValueGeneratedNever();

        b.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.Color)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.StartTime)
            .HasConversion(IdConverters.TimeOnlyToTimeSpan)
            .IsRequired();

        b.Property(x => x.EndTime)
            .HasConversion(IdConverters.TimeOnlyToTimeSpan)
            .IsRequired();

        b.Property(x => x.Date)
            .HasConversion(IdConverters.DateOnlyToDateTime)
            .IsRequired();

        b.Property(x => x.EndDate)
            .HasConversion(IdConverters.NullableDateOnlyToDateTime)
            .IsRequired(false);

        b.Property(x => x.Location)
            .HasMaxLength(256);

        b.Property(x => x.GoogleEventId)
            .HasMaxLength(256);

        b.Property(x => x.TaskId)
            .HasConversion(IdConverters.NullableGuidId(
                id => id == null ? null : id.Id,
                value => value == null ? null : new TodoItemId(value.Value)))
            .IsRequired(false);

        b.HasOne(x => x.Task)
            .WithOne(x => x.CalendarEvent)
            .HasForeignKey<CalendarEventEntity>(x => x.TaskId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(x => x.Date);
        b.HasIndex(x => x.Type);
    }
}