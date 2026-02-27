using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Persistence.Configurations.Converters;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class TodoItemEntityConfiguration : IEntityTypeConfiguration<TodoItemEntity>
{
    public void Configure(EntityTypeBuilder<TodoItemEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasConversion(IdConverters.GuidId<TodoItemId>(
                id => id.Id,
                value => new TodoItemId(value)))
            .ValueGeneratedNever();

        b.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.TaskStatus)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.TaskPriority)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.CreatorId)
            .HasConversion(IdConverters.GuidId(
                id => id.Id,
                value => new UserId(value)))
            .IsRequired();

        b.Property(x => x.AssigneeId)
            .HasConversion(IdConverters.NullableGuidId(
                id => id == null ? null : id.Id,
                value => value == null ? null : new UserId(value.Value)))
            .IsRequired(false);

        b.Property(x => x.ColumnId)
            .HasConversion(IdConverters.GuidId(
                id => id.Id,
                value => new ColumnId(value)))
            .IsRequired();

        b.Property(x => x.CreatedAtUtc).IsRequired();
        b.Property(x => x.UpdatedAtUtc);
        b.Property(x => x.DeadlineUtc);

        b.Property(x => x.ProgressPct);

        b.Property(x => x.BacklogId)
            .HasConversion(IdConverters.NullableGuidId(
                id => id == null ? null : id.Id,
                value => value == null ? null : new BacklogId(value.Value)))
            .IsRequired(false);

        b.Property(x => x.DeadlineText)
            .HasMaxLength(128);

        b.Property(x => x.CompletedText)
            .HasMaxLength(128);

        b.HasIndex(x => x.ColumnId);
        b.HasIndex(x => x.CreatorId);
        b.HasIndex(x => x.AssigneeId);
        b.HasIndex(x => x.TaskStatus);
        b.HasIndex(x => x.DeadlineUtc);
    }
}