using Microsoft.EntityFrameworkCore;
using Orbita.Domain.Entities;

namespace Orbita.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(user => user.Id);
            builder.Property(user => user.Email).HasMaxLength(256).IsRequired();
            builder.HasIndex(user => user.Email).IsUnique();
            builder.Property(user => user.PasswordHash).HasMaxLength(512).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
