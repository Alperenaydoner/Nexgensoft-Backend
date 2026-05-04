using CoreService.Content.Domain.Entities;

namespace CoreService.Content.Infrastructure.Repositories;

public interface ISiteContentRepository
{
    Task<SiteContentBundleEntity?> GetByLocaleAsync(string locale, CancellationToken cancellationToken = default);

    Task<bool> HasAnyBundlesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, string>> GetStringDictionaryAsync(string locale, CancellationToken cancellationToken = default);

    Task<bool> HasAnyStringsAsync(CancellationToken cancellationToken = default);
}
