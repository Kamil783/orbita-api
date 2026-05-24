using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class PlannedPurchaseEntityConfiguration : IEntityTypeConfiguration<PlannedPurchaseEntity>
{
    public void Configure(EntityTypeBuilder<PlannedPurchaseEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever();

        b.Property(x => x.OwnerId)
            .IsRequired();

        b.Property(x => x.TeamId)
            .IsRequired();

        b.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Date)
            .IsRequired();

        b.Property(x => x.Amount)
            .IsRequired();

        b.Property(x => x.AssigneeKind)
            .HasConversion<int?>()
            .IsRequired(false);

        b.Property(x => x.AssigneeUserId)
            .IsRequired(false);

        b.Property(x => x.CategoryId)
            .IsRequired(false);

        b.Property(x => x.Note)
            .HasMaxLength(1000)
            .IsRequired(false);

        b.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .IsRequired();

        b.Property(x => x.UpdatedAt)
            .IsRequired();

        b.HasIndex(x => x.TeamId);
        b.HasIndex(x => new { x.TeamId, x.Date });
        b.HasIndex(x => x.AssigneeUserId);
        b.HasIndex(x => x.AssigneeKind);
    }
}
