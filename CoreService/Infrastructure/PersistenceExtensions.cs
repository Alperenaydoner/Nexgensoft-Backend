using CoreService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Infrastructure;

public static class PersistenceExtensions
{
    public static IServiceCollection AddSqlServerPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
