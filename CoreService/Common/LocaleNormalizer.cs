namespace CoreService.Common;

/// <summary>API ve içerik için tr/en yerel ayarını tek yerden normalize eder.</summary>
public static class LocaleNormalizer
{
    public static string NormalizeTrEn(string? locale)
    {
        var l = string.IsNullOrWhiteSpace(locale) ? "tr" : locale.Trim().ToLowerInvariant();
        if (l.Length > 2)
        {
            l = l[..2];
        }

        return l is "tr" or "en" ? l : "tr";
    }
}
