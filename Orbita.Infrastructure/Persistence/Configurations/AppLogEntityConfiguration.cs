using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Persistence.Configurations;

public class AppLogEntityConfiguration : IEntityTypeConfiguration<AppLogEntity>
{
    public void Configure(EntityTypeBuilder<AppLogEntity> builder)
    {
        builder.ToTable("AppLogs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Level).HasMaxLength(32);
        builder.Property(x => x.Message).HasColumnType("text");
        builder.Property(x => x.Exception).HasColumnType("text");
        builder.Property(x => x.StackTrace).HasColumnType("text");
        builder.Property(x => x.Source).HasMaxLength(512);
        builder.Property(x => x.TraceId).HasMaxLength(128);

        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.Level);
        builder.HasIndex(x => x.TraceId);
    }
}
