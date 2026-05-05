namespace CoreService.Audit.Domain.Entities;

/// <summary>
/// API HTTP istek audit satırı. Kimlik doğrulaması olmayan trafik için <see cref="UserId"/> vb. alanlar boş olabilir.
/// </summary>
public class HttpRequestLog
{
    public Guid Id { get; set; }

    public DateTime OccurredAtUtc { get; set; }

    /// <summary>JWT / IdP subject veya dahili kullanıcı kimliği (isteğe bağlı).</summary>
    public string? UserId { get; set; }

    public string? UserEmail { get; set; }

    public string? UserRoles { get; set; }

    public string HttpMethod { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string? QueryString { get; set; }

    public string? ClientIp { get; set; }

    public string? UserAgent { get; set; }

    public string? AcceptLanguage { get; set; }

    public string? Referer { get; set; }

    public string? CorrelationId { get; set; }

    public string? TraceId { get; set; }

    public string? EnvironmentName { get; set; }

    public string? EndpointController { get; set; }

    public string? EndpointAction { get; set; }

    public int StatusCode { get; set; }

    public long DurationMs { get; set; }

    public bool Success { get; set; }

    public string? ExceptionType { get; set; }

    public string? ExceptionMessage { get; set; }

    /// <summary>İsteğe bağlı maskele/truncate edilmiş gövde özeti (JSON vb.).</summary>
    public string? RequestBodySnippet { get; set; }

    /// <summary><c>AuditAction</c> işlem kodu (Mapa: <c>IslemTuru</c>).</summary>
    public string? ActionType { get; set; }

    /// <summary>İşlem başlığı (şablon çözülmüş).</summary>
    public string? ActionTitle { get; set; }

    /// <summary>İşlem açıklaması (şablon çözülmüş).</summary>
    public string? ActionDescription { get; set; }
}
