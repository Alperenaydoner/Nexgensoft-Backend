using CoreService.Audit.DTOs;
using CoreService.Audit.Infrastructure;
using CoreService.Audit.Infrastructure.Repositories;
using CoreService.Audit.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreService.Audit;

public static class AuditFeatureExtensions
{
    public static IServiceCollection AddAuditFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuditOptions>(configuration.GetSection(AuditOptions.SectionName));
        services.AddScoped<IHttpRequestLogRepository, HttpRequestLogRepository>();
        services.AddScoped<IRequestAuditLogger, RequestAuditLogger>();
        return services;
    }

    public static IApplicationBuilder UseRequestAudit(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestAuditMiddleware>();
    }
}
