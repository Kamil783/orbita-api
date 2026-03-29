using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class SavingsGoalEntityConfiguration : IEntityTypeConfiguration<SavingsGoalEntity>
{
    public void Configure(EntityTypeBuilder<SavingsGoalEntity> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever();

        b.Property(x => x.CreatorId)
            .IsRequired();

        b.Property(x => x.TeamId)
            .IsRequired();

        b.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Target)
            .IsRequired();

        b.Property(x => x.Current)
            .IsRequired();

        b.HasIndex(x => x.TeamId);
    }
}
