using CoreService.Common;
using CoreService.Content.DTOs.Responses;
using CoreService.Content.Infrastructure.Data;
using CoreService.Content.Infrastructure.Repositories;
using CoreService.Content.Mapping;

namespace CoreService.Content.Services;

public class SiteContentService(ISiteContentRepository repository) : ISiteContentService
{
    public async Task<SiteContentBundle> GetBundleAsync(string? locale, CancellationToken cancellationToken = default)
    {
        var normalized = LocaleNormalizer.NormalizeTrEn(locale);
        var flat = await repository.GetStringDictionaryAsync(normalized, cancellationToken);
        var translation = flat.Count > 0
            ? SiteContentTranslationBuilder.ToJsonElement(flat)
            : SiteContentJsonMapper.EmptyTranslation;

        var row = await repository.GetByLocaleAsync(normalized, cancellationToken);
        var layout = row is null ? null : SiteContentJsonMapper.DeserializeLayout(row.PayloadJson);
        return SiteContentJsonMapper.Combine(layout, normalized, translation);
    }
}
