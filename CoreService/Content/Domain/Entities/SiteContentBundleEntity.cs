using CoreService.Common;

namespace CoreService.Content.Domain.Entities;

/// <summary>
/// Tek satır = bir yerel ayar için tüm site içerik paketi (<c>SiteContentBundle</c> JSON).
/// Nav / sayfa / blok ayrı tablolar yerine parametrik payload.
/// </summary>
public class SiteContentBundleEntity : BaseEntity<int>
{
    /// <summary>tr, en</summary>
    public string Locale { get; set; } = string.Empty;

    /// <summary>SiteContentBundle (DTO) ile uyumlu JSON.</summary>
    public string PayloadJson { get; set; } = "{}";
}
