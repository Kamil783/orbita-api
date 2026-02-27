using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class UserProfileEntityConfiguration : IEntityTypeConfiguration<UserProfileEntity>
{
    public void Configure(EntityTypeBuilder<UserProfileEntity> b)
    {
        b.HasKey(p => p.UserId);

        b.Property(p => p.Name)
            .HasMaxLength(200);

        b.Property(p => p.AvatarData)
            .HasColumnType("bytea");

        b.Property(p => p.AvatarContentType)
            .HasMaxLength(100);

        b.HasOne(p => p.User)
            .WithOne(u => u.UserProfile)
            .HasForeignKey<UserProfileEntity>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
