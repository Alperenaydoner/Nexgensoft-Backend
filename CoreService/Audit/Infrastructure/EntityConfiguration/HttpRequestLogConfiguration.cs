using CoreService.Audit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreService.Audit.Infrastructure.EntityConfiguration;

public class HttpRequestLogConfiguration : IEntityTypeConfiguration<HttpRequestLog>
{
    public void Configure(EntityTypeBuilder<HttpRequestLog> builder)
    {
        builder.ToTable("http_request_logs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OccurredAtUtc).IsRequired();

        builder.Property(x => x.UserId).HasMaxLength(128);
        builder.Property(x => x.UserEmail).HasMaxLength(320);
        builder.Property(x => x.UserRoles).HasMaxLength(512);

        builder.Property(x => x.HttpMethod).HasMaxLength(16).IsRequired();
        builder.Property(x => x.Path).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.QueryString).HasMaxLength(2048);

        builder.Property(x => x.ClientIp).HasMaxLength(64);
        builder.Property(x => x.UserAgent).HasMaxLength(512);
        builder.Property(x => x.AcceptLanguage).HasMaxLength(64);
        builder.Property(x => x.Referer).HasMaxLength(1024);

        builder.Property(x => x.CorrelationId).HasMaxLength(64);
        builder.Property(x => x.TraceId).HasMaxLength(64);
        builder.Property(x => x.EnvironmentName).HasMaxLength(32);

        builder.Property(x => x.EndpointController).HasMaxLength(128);
        builder.Property(x => x.EndpointAction).HasMaxLength(128);

        builder.Property(x => x.ExceptionType).HasMaxLength(256);
        builder.Property(x => x.ExceptionMessage).HasMaxLength(2000);
        builder.Property(x => x.RequestBodySnippet).HasColumnType("text");

        builder.Property(x => x.ActionType).HasMaxLength(150);
        builder.Property(x => x.ActionTitle).HasMaxLength(300);
        builder.Property(x => x.ActionDescription).HasMaxLength(2000);

        builder.HasIndex(x => x.OccurredAtUtc);
        builder.HasIndex(x => x.ClientIp);
        builder.HasIndex(x => x.HttpMethod);
        builder.HasIndex(x => x.StatusCode);
        builder.HasIndex(x => new { x.HttpMethod, x.OccurredAtUtc });
        builder.HasIndex(x => new { x.StatusCode, x.OccurredAtUtc });
        builder.HasIndex(x => new { x.Path, x.OccurredAtUtc });
        builder.HasIndex(x => x.ActionType);
    }
}
