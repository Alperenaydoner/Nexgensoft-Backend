using System.Text.Json.Serialization;

namespace CoreService.Admin.DTOs;

public sealed class AdminStatsDto
{
    [JsonPropertyName("userCount")]
    public int UserCount { get; init; }

    [JsonPropertyName("roleCount")]
    public int RoleCount { get; init; }

    [JsonPropertyName("contactMessageCount")]
    public int ContactMessageCount { get; init; }

    [JsonPropertyName("contactAttachmentCount")]
    public int ContactAttachmentCount { get; init; }

    [JsonPropertyName("httpRequestLogCount")]
    public int HttpRequestLogCount { get; init; }

    [JsonPropertyName("siteContentBundleCount")]
    public int SiteContentBundleCount { get; init; }

    [JsonPropertyName("siteLocalizedStringCount")]
    public int SiteLocalizedStringCount { get; init; }
}

public class AdminUserListItemDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; init; } = string.Empty;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

    [JsonPropertyName("createdAtUtc")]
    public DateTime CreatedAtUtc { get; init; }

    [JsonPropertyName("roles")]
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
}

public sealed class AdminUserDetailDto : AdminUserListItemDto
{
    [JsonPropertyName("normalizedEmail")]
    public string NormalizedEmail { get; init; } = string.Empty;
}

public sealed class AdminRoleListItemDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("normalizedName")]
    public string NormalizedName { get; init; } = string.Empty;

    [JsonPropertyName("userCount")]
    public int UserCount { get; init; }
}

public sealed class AdminContactMessageListItemDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("fullName")]
    public string FullName { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("company")]
    public string? Company { get; init; }

    [JsonPropertyName("createdAtUtc")]
    public DateTime CreatedAtUtc { get; init; }

    [JsonPropertyName("attachmentCount")]
    public int AttachmentCount { get; init; }
}

public sealed class AdminContactAttachmentDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("originalFileName")]
    public string OriginalFileName { get; init; } = string.Empty;

    [JsonPropertyName("contentType")]
    public string ContentType { get; init; } = string.Empty;

    [JsonPropertyName("sizeBytes")]
    public long SizeBytes { get; init; }

    [JsonPropertyName("isImage")]
    public bool IsImage { get; init; }
}

public sealed class AdminContactMessageDetailDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("fullName")]
    public string FullName { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("company")]
    public string? Company { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;

    [JsonPropertyName("createdAtUtc")]
    public DateTime CreatedAtUtc { get; init; }

    [JsonPropertyName("attachments")]
    public IReadOnlyList<AdminContactAttachmentDto> Attachments { get; init; } = Array.Empty<AdminContactAttachmentDto>();
}

public class AdminHttpRequestLogListItemDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("occurredAtUtc")]
    public DateTime OccurredAtUtc { get; init; }

    [JsonPropertyName("httpMethod")]
    public string HttpMethod { get; init; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; init; } = string.Empty;

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; init; }

    [JsonPropertyName("durationMs")]
    public long DurationMs { get; init; }

    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("userEmail")]
    public string? UserEmail { get; init; }

    [JsonPropertyName("actionType")]
    public string? ActionType { get; init; }
}

public sealed class AdminHttpRequestLogDetailDto : AdminHttpRequestLogListItemDto
{
    [JsonPropertyName("queryString")]
    public string? QueryString { get; init; }

    [JsonPropertyName("clientIp")]
    public string? ClientIp { get; init; }

    [JsonPropertyName("userAgent")]
    public string? UserAgent { get; init; }

    [JsonPropertyName("acceptLanguage")]
    public string? AcceptLanguage { get; init; }

    [JsonPropertyName("referer")]
    public string? Referer { get; init; }

    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; init; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; init; }

    [JsonPropertyName("environmentName")]
    public string? EnvironmentName { get; init; }

    [JsonPropertyName("endpointController")]
    public string? EndpointController { get; init; }

    [JsonPropertyName("endpointAction")]
    public string? EndpointAction { get; init; }

    [JsonPropertyName("exceptionType")]
    public string? ExceptionType { get; init; }

    [JsonPropertyName("exceptionMessage")]
    public string? ExceptionMessage { get; init; }

    [JsonPropertyName("requestBodySnippet")]
    public string? RequestBodySnippet { get; init; }

    [JsonPropertyName("actionTitle")]
    public string? ActionTitle { get; init; }

    [JsonPropertyName("actionDescription")]
    public string? ActionDescription { get; init; }

    [JsonPropertyName("userId")]
    public string? UserId { get; init; }

    [JsonPropertyName("userRoles")]
    public string? UserRoles { get; init; }
}

public sealed class AdminContentLocaleRowDto
{
    [JsonPropertyName("locale")]
    public string Locale { get; init; } = string.Empty;

    [JsonPropertyName("hasBundle")]
    public bool HasBundle { get; init; }

    [JsonPropertyName("localizedStringCount")]
    public int LocalizedStringCount { get; init; }
}

public sealed class AdminContentOverviewDto
{
    [JsonPropertyName("bundleCount")]
    public int BundleCount { get; init; }

    [JsonPropertyName("totalLocalizedStrings")]
    public int TotalLocalizedStrings { get; init; }

    [JsonPropertyName("locales")]
    public IReadOnlyList<AdminContentLocaleRowDto> Locales { get; init; } = Array.Empty<AdminContentLocaleRowDto>();
}
