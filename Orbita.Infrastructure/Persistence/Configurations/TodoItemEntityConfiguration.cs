using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class TodoItemEntityConfiguration : IEntityTypeConfiguration<TodoItemEntity>
{
    public void Configure(EntityTypeBuilder<TodoItemEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
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
            .IsRequired();

        b.Property(x => x.AssigneeId)
            .IsRequired(false);

        b.Property(x => x.ColumnId)
            .IsRequired();

        b.Property(x => x.CreatedAtUtc).IsRequired();
        b.Property(x => x.UpdatedAtUtc);
        b.Property(x => x.DeadlineUtc);

        b.Property(x => x.ProgressPct);

        b.Property(x => x.BacklogId)
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
