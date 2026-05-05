using CoreService.Application.DTOs;
using CoreService.Application.Infrastructure.Repositories;
using CoreService.Application.Services;

namespace CoreService.Application;

public static class ApplicationFeatureExtensions
{
    public static IServiceCollection AddApplicationFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApplicationUploadOptions>(configuration.GetSection(ApplicationUploadOptions.SectionName));
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IApplicationService, ApplicationService>();
        return services;
    }
}
