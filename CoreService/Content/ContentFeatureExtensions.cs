using CoreService.Content.Infrastructure;
using CoreService.Content.Infrastructure.Repositories;
using CoreService.Content.Services;

namespace CoreService.Content;

public static class ContentFeatureExtensions
{
    public static IServiceCollection AddContentFeature(this IServiceCollection services)
    {
        services.AddScoped<ISiteContentRepository, SiteContentRepository>();
        services.AddScoped<ISiteContentService, SiteContentService>();
        services.AddScoped<ISiteContentSeeder, SiteContentSeeder>();
        return services;
    }
}
