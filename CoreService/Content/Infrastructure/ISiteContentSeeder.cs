namespace CoreService.Content.Infrastructure;

/// <summary>Yapılandırmaya göre idempotent site içeriği tohumlar.</summary>
public interface ISiteContentSeeder
{
    Task SeedIfEmptyAsync(CancellationToken cancellationToken = default);
}
