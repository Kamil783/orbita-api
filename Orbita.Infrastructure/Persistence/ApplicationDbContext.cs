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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrbitaDbContext).Assembly);
    }
}
