using System.Text.Json;
using CoreService.Common.Serialization;
using CoreService.Content.DTOs.Responses;

namespace CoreService.Content.Mapping;

public static class SiteContentJsonMapper
{
    public static SiteContentLayoutPayload? DeserializeLayout(string payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            return null;
        }

        return JsonSerializer.Deserialize<SiteContentLayoutPayload>(payloadJson, ApiJsonSerializerOptions.CamelCase);
    }

    public static SiteContentBundle Combine(
        SiteContentLayoutPayload? layout,
        string localeFallback,
        JsonElement translation)
    {
        if (layout is null)
        {
            return new SiteContentBundle(
                localeFallback,
                [],
                new Dictionary<string, StaticPageDocument>(StringComparer.OrdinalIgnoreCase),
                translation);
        }

        return new SiteContentBundle(
            layout.Locale,
            layout.Navigation,
            layout.Pages,
            translation);
    }

    public static JsonElement EmptyTranslation =>
        JsonSerializer.SerializeToElement(new { }, ApiJsonSerializerOptions.CamelCase);
}
