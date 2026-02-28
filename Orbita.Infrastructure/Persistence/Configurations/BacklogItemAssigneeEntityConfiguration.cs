using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities.Mapping;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class BacklogItemAssigneeEntityConfiguration : IEntityTypeConfiguration<BacklogTaskAssigneeEntity>
{
    public void Configure(EntityTypeBuilder<BacklogTaskAssigneeEntity> b)
    {
        b.HasKey(x => new { x.BacklogTaskId, x.UserId });

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
