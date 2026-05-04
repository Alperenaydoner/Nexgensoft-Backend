using System.Text.Json;
using CoreService.Common.Serialization;
using CoreService.Content.Domain.Entities;
using CoreService.Content.DTOs.Responses;

namespace CoreService.Content.Infrastructure.Data;

/// <summary>Varsayılan site yerleşimi (nav + sayfalar) JSON satırları.</summary>
public static class SiteContentSeedData
{
    public static IReadOnlyList<SiteContentBundleEntity> BuildSeedRows()
    {
        var tr = BuildTrLayout();
        var en = BuildEnLayout();
        return
        [
            new SiteContentBundleEntity
            {
                Locale = tr.Locale,
                PayloadJson = JsonSerializer.Serialize(tr, ApiJsonSerializerOptions.CamelCase),
            },
            new SiteContentBundleEntity
            {
                Locale = en.Locale,
                PayloadJson = JsonSerializer.Serialize(en, ApiJsonSerializerOptions.CamelCase),
            },
        ];
    }

    private static SiteContentLayoutPayload BuildTrLayout()
    {
        var locale = "tr";
        return new SiteContentLayoutPayload
        {
            Locale = locale,
            Navigation =
            [
                new("", "Ana Sayfa", 0),
                new("about", "Hakkımızda", 1),
                new("services", "Hizmetler", 2),
                new("application", "Başvuru", 3),
                new("contact", "İletişim", 4),
            ],
            Pages = new Dictionary<string, StaticPageDocument>(StringComparer.OrdinalIgnoreCase)
            {
                ["home"] = new(
                    "home",
                    locale,
                    "Ana Sayfa",
                    "Nexgensoft — yazılım ve danışmanlık.",
                    [new("heroTitle", "Nexgensoft'a hoş geldiniz"), new("heroLead", "Modern web ve bulut çözümleri.")]),
                ["about"] = new(
                    "about",
                    locale,
                    "Hakkımızda",
                    null,
                    [new("intro", "Şirketimiz hakkında kısa bilgi (örnek içerik).")]),
            },
        };
    }

    private static SiteContentLayoutPayload BuildEnLayout()
    {
        var locale = "en";
        return new SiteContentLayoutPayload
        {
            Locale = locale,
            Navigation =
            [
                new("", "Home", 0),
                new("about", "About", 1),
                new("services", "Services", 2),
                new("application", "Apply", 3),
                new("contact", "Contact", 4),
            ],
            Pages = new Dictionary<string, StaticPageDocument>(StringComparer.OrdinalIgnoreCase)
            {
                ["home"] = new(
                    "home",
                    locale,
                    "Home",
                    "Nexgensoft — software and consulting.",
                    [new("heroTitle", "Welcome to Nexgensoft"), new("heroLead", "Modern web and cloud solutions.")]),
                ["about"] = new(
                    "about",
                    locale,
                    "About",
                    null,
                    [new("intro", "Short about text (sample content).")]),
            },
        };
    }
}
