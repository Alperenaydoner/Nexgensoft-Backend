using CoreService.Admin.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CoreService.Admin;

public static class AdminFeatureExtensions
{
    public static IServiceCollection AddAdminFeature(this IServiceCollection services)
    {
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();
        return services;
    }
}
