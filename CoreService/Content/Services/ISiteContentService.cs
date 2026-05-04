using CoreService.Content.DTOs.Responses;

namespace CoreService.Content.Services;

public interface ISiteContentService
{
    Task<SiteContentBundle> GetBundleAsync(string? locale, CancellationToken cancellationToken = default);
}
