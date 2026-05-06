using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoreService.Application.DTOs.Requests;

public class ApplicationAttachmentSubmitDto
{
    [Required]
    [MaxLength(260)]
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;

    [MaxLength(128)]
    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }

    [Required]
    [JsonPropertyName("base64")]
    public string Base64 { get; set; } = string.Empty;
}

public class ApplicationSubmitRequest
{
    [Required]
    [MaxLength(200)]
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(320)]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(32)]
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [Required]
    [MaxLength(200)]
    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;

    [MaxLength(8000)]
    [JsonPropertyName("coverLetter")]
    public string? CoverLetter { get; set; }

    [JsonPropertyName("attachments")]
    public List<ApplicationAttachmentSubmitDto>? Attachments { get; set; }
}

public class ApplicationUpdateByCodeRequest
{
    [MaxLength(200)]
    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [EmailAddress]
    [MaxLength(320)]
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [MaxLength(32)]
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [MaxLength(200)]
    [JsonPropertyName("position")]
    public string? Position { get; set; }

    [MaxLength(8000)]
    [JsonPropertyName("coverLetter")]
    public string? CoverLetter { get; set; }

    [JsonPropertyName("attachments")]
    public List<ApplicationAttachmentSubmitDto>? Attachments { get; set; }
}
