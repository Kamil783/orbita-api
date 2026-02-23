using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Mapping;

public class UserProfileMapping : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> b)
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
            .HasForeignKey<UserProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
