using System.Text.Json;

namespace CoreService.Content.DTOs.Responses;

/// <summary>Veritabanında saklanan site yerleşimi (nav + sayfalar) — translation ayrı tablodan birleştirilir.</summary>
public class SiteContentLayoutPayload
{
    public string Locale { get; set; } = string.Empty;

    public List<SiteNavigationItem> Navigation { get; set; } = [];

    public Dictionary<string, StaticPageDocument> Pages { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public record StaticPageBlock(string Key, string Content);

public record StaticPageDocument(
    string Slug,
    string Locale,
    string Title,
    string? MetaDescription,
    IReadOnlyList<StaticPageBlock> Blocks);

public record SiteNavigationItem(string Slug, string Label, int Order);

/// <summary>GET /api/v1/content/site cevabı — <c>translation</c> i18next resource ile uyumlu iç içe nesne.</summary>
public record SiteContentBundle(
    string Locale,
    IReadOnlyList<SiteNavigationItem> Navigation,
    IReadOnlyDictionary<string, StaticPageDocument> Pages,
    JsonElement Translation);
