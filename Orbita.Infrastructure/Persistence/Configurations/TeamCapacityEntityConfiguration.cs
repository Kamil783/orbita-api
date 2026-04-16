using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class TeamCapacityEntityConfiguration : IEntityTypeConfiguration<TeamCapacityEntity>
{
    public void Configure(EntityTypeBuilder<TeamCapacityEntity> b)
    {
        b.HasKey(x => x.TeamId);

        b.Property(x => x.TeamId)
            .ValueGeneratedNever();

        b.Property(x => x.WeekdayHours)
            .IsRequired();

        b.Property(x => x.WeekendHours)
            .IsRequired();
    }
}
