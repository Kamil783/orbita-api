using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence;

public class OrbitaDbContext(DbContextOptions<OrbitaDbContext> options) : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();
    public DbSet<RequestLogEntity> RequestLogs => Set<RequestLogEntity>();
    public DbSet<AppLogEntity> AppLogs => Set<AppLogEntity>();
    public DbSet<UserProfileEntity> UserProfiles => Set<UserProfileEntity>();
    public DbSet<AppNotificationEntity> AppNotifications => Set<AppNotificationEntity>();
    public DbSet<BacklogTaskEntity> BacklogTasks => Set<BacklogTaskEntity>();
    public DbSet<CalendarEventEntity> CalendarEvents => Set<CalendarEventEntity>();
    public DbSet<ColumnEntity> Columns => Set<ColumnEntity>();
    public DbSet<TodoItemEntity> TodoItems => Set<TodoItemEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrbitaDbContext).Assembly);
    }
}
