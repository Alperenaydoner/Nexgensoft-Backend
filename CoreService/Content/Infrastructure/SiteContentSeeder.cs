using System.Reflection;
using CoreService.Content.Domain.Entities;
using CoreService.Content.Infrastructure.Data;
using CoreService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Content.Infrastructure;

public class SiteContentSeeder(AppDbContext db, ILogger<SiteContentSeeder> logger) : ISiteContentSeeder
{
    public async Task SeedIfEmptyAsync(CancellationToken cancellationToken = default)
    {
        await SeedLocalizedStringsIfEmptyAsync(cancellationToken);

        if (await db.SiteContentBundles.AnyAsync(cancellationToken))
        {
            return;
        }

        logger.LogInformation("site_content_bundles boş; varsayılan yerleşim JSON ekleniyor.");

        db.SiteContentBundles.AddRange(SiteContentSeedData.BuildSeedRows());
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Site yerleşim tohumları tamamlandı.");
    }

    private async Task SeedLocalizedStringsIfEmptyAsync(CancellationToken cancellationToken)
    {
        if (await db.SiteLocalizedStrings.AnyAsync(cancellationToken))
        {
            return;
        }

        logger.LogInformation("site_localized_strings boş; SeedLocales JSON gömülü kaynaklardan flatten ediliyor.");

        var asm = typeof(SiteContentSeeder).Assembly;
        foreach (var loc in new[] { "tr", "en" })
        {
            var resourceName = $"{asm.GetName().Name}.Content.Infrastructure.Data.SeedLocales.{loc}.json";
            await using var stream = asm.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                var available = string.Join(", ", asm.GetManifestResourceNames());
                throw new InvalidOperationException($"Gömülü locale bulunamadı: {resourceName}. Kaynaklar: {available}");
            }

            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
            foreach (var (key, value) in JsonTranslationFlattener.Flatten(json))
            {
                db.SiteLocalizedStrings.Add(new SiteLocalizedStringEntity
                {
                    Locale = loc,
                    StringKey = key,
                    Content = value,
                });
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("site_localized_strings tohumları tamamlandı.");
    }
}
