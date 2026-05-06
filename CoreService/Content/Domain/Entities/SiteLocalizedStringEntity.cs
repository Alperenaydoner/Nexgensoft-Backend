using System.ComponentModel.DataAnnotations;
using CoreService.Common;

namespace CoreService.Content.Domain.Entities;

/// <summary>Statik metin satırı — Mapa StaticText benzeri Key + Content + dil.</summary>
public class SiteLocalizedStringEntity : BaseEntity<int>
{
    /// <summary>tr, en</summary>
    [Required]
    [MaxLength(10)]
    public string Locale { get; set; } = string.Empty;

    /// <summary>Nokta notasyonlu anahtar (örn. nav.home, home.stats.0.label).</summary>
    [Required]
    [MaxLength(250)]
    public string StringKey { get; set; } = string.Empty;

    /// <summary>Görünen metin.</summary>
    [Required]
    public string Content { get; set; } = string.Empty;
}
