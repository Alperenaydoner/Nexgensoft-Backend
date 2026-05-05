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

    public Task<int> CountBundlesAsync(CancellationToken cancellationToken = default) =>
        db.SiteContentBundles.AsNoTracking().CountAsync(cancellationToken);

    public Task<int> CountLocalizedStringsAsync(CancellationToken cancellationToken = default) =>
        db.SiteLocalizedStrings.AsNoTracking().CountAsync(cancellationToken);

    public async Task<IReadOnlyList<(string Locale, int Count)>> GetLocalizedStringCountsByLocaleAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.SiteLocalizedStrings.AsNoTracking()
            .GroupBy(x => x.Locale)
            .Select(g => new { Locale = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(r => (r.Locale, r.Count)).ToList();
    }

    public async Task<IReadOnlyList<string>> GetBundleLocalesAsync(CancellationToken cancellationToken = default)
    {
        return await db.SiteContentBundles.AsNoTracking()
            .Select(x => x.Locale)
            .OrderBy(x => x)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<SiteLocalizedStringEntity>> GetLocalizedStringsByLocaleAsync(
        string locale,
        CancellationToken cancellationToken = default)
    {
        return await db.SiteLocalizedStrings.AsNoTracking()
            .Where(x => x.Locale == locale)
            .OrderBy(x => x.StringKey)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task UpsertLocalizedStringsAsync(
        string locale,
        IReadOnlyList<SiteLocalizedStringEntity> rows,
        CancellationToken cancellationToken = default)
    {
        var existing = await db.SiteLocalizedStrings
            .Where(x => x.Locale == locale)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        db.SiteLocalizedStrings.RemoveRange(existing);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (rows.Count == 0)
        {
            return;
        }

        db.SiteLocalizedStrings.AddRange(rows);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
