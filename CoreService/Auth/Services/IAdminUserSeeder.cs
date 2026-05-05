namespace CoreService.Auth.Services;

public interface IAdminUserSeeder
{
    Task SeedAdminIfEnabledAsync(CancellationToken cancellationToken = default);
}
