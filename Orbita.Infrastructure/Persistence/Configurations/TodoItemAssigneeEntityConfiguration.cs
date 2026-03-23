using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities.Mapping;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class TodoItemAssigneeEntityConfiguration : IEntityTypeConfiguration<TodoItemAssigneeEntity>
{
    public void Configure(EntityTypeBuilder<TodoItemAssigneeEntity> b)
    {
        b.HasKey(x => new { x.TodoItemId, x.UserId });

        b.HasOne(x => x.TodoItem)
               .WithMany(t => t.Assignees)
               .HasForeignKey(x => x.TodoItemId)
               .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.UserProfile)
               .WithMany(u => u.AssignedTodoItems)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.UserId);
    }
}
