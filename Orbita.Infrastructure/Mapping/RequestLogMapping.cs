using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Mapping;

public class RequestLogMapping : IEntityTypeConfiguration<RequestLogEntity>
{
    public void Configure(EntityTypeBuilder<RequestLogEntity> builder)
    {
        builder.ToTable("RequestLogs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TraceId).HasMaxLength(128);
        builder.Property(x => x.HttpMethod).HasMaxLength(16);
        builder.Property(x => x.Path).HasMaxLength(2048);
        builder.Property(x => x.QueryString).HasMaxLength(2048);
        builder.Property(x => x.RequestBody).HasColumnType("text");
        builder.Property(x => x.RequestHeaders).HasColumnType("text");
        builder.Property(x => x.ResponseBody).HasColumnType("text");
        builder.Property(x => x.UserId).HasMaxLength(256);
        builder.Property(x => x.ClientIp).HasMaxLength(64);
        builder.Property(x => x.UserAgent).HasMaxLength(512);
        builder.Property(x => x.ControllerName).HasMaxLength(256);
        builder.Property(x => x.ActionName).HasMaxLength(256);

        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.TraceId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Path);
    }
}
