namespace CoreService.Audit.DTOs;

public class AuditOptions
{
    public const string SectionName = "Audit";

    /// <summary>HTTP istek audit kaydı tutulsun mu.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>JSON gövde kaydı (varsayılan kapalı; multipart / büyük isteklerde kapatın).</summary>
    public bool LogRequestBody { get; set; }

    /// <summary><see cref="LogRequestBody"/> açıksa saklanacak en fazla karakter.</summary>
    public int MaxRequestBodyCharsToStore { get; set; } = 2000;

    /// <summary>Bu öneklerle başlayan path'ler loglanmaz (ör. <c>/health</c>).</summary>
    public string[] ExcludedPathPrefixes { get; set; } = [];
}
