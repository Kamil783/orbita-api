using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities.Mapping;
using Orbita.Infrastructure.Persistence.Configurations.Converters;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class BacklogItemAssigneeEntityConfiguration : IEntityTypeConfiguration<BacklogTaskAssigneeEntity>
{
    public void Configure(EntityTypeBuilder<BacklogTaskAssigneeEntity> b)
    {
        var userIdConverter = IdConverters.GuidId(x => x.Id, g => new UserId(g));
        var backlogIdConverter = IdConverters.GuidId(x => x.Id, g => new BacklogId(g));


        b.HasKey(x => new { x.BacklogTaskId, x.UserId });

        b.Property(x => x.BacklogTaskId)
               .HasConversion(backlogIdConverter);

        b.Property(x => x.UserId)
               .HasConversion(userIdConverter);

        b.HasOne(x => x.BacklogTask)
               .WithMany(t => t.Assignees)
               .HasForeignKey(x => x.BacklogTaskId)
               .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.UserProfile)
               .WithMany(u => u.AssignedBacklogTaskItems)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.UserId);
    }
}
