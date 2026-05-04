using CoreService.Content.Domain.Entities;
using CoreService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Content.Infrastructure.Repositories;

public class SiteContentRepository(AppDbContext db) : ISiteContentRepository
{
    public Task<SiteContentBundleEntity?> GetByLocaleAsync(string locale, CancellationToken cancellationToken = default) =>
        db.SiteContentBundles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Locale == locale, cancellationToken);

    public Task<bool> HasAnyBundlesAsync(CancellationToken cancellationToken = default) =>
        db.SiteContentBundles.AsNoTracking().AnyAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<string, string>> GetStringDictionaryAsync(
        string locale,
        CancellationToken cancellationToken = default)
    {
        return await db.SiteLocalizedStrings.AsNoTracking()
            .Where(x => x.Locale == locale)
            .ToDictionaryAsync(x => x.StringKey, x => x.Content, StringComparer.Ordinal, cancellationToken);
    }

    public Task<bool> HasAnyStringsAsync(CancellationToken cancellationToken = default) =>
        db.SiteLocalizedStrings.AsNoTracking().AnyAsync(cancellationToken);
}
