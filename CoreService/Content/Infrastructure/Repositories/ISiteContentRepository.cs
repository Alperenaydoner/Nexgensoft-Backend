using CoreService.Content.Domain.Entities;

namespace CoreService.Content.Infrastructure.Repositories;

public interface ISiteContentRepository
{
    Task<SiteContentBundleEntity?> GetByLocaleAsync(string locale, CancellationToken cancellationToken = default);

    Task<bool> HasAnyBundlesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, string>> GetStringDictionaryAsync(string locale, CancellationToken cancellationToken = default);

    Task<bool> HasAnyStringsAsync(CancellationToken cancellationToken = default);

    Task<int> CountBundlesAsync(CancellationToken cancellationToken = default);

    Task<int> CountLocalizedStringsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(string Locale, int Count)>> GetLocalizedStringCountsByLocaleAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetBundleLocalesAsync(CancellationToken cancellationToken = default);
}
