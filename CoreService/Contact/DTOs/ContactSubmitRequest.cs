using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoreService.Contact.DTOs;

public class ContactAttachmentSubmitDto
{
    [Required]
    [MaxLength(260)]
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;

    [MaxLength(128)]
    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }

    /// <summary>Ham Base64 (data:…;base64, öneki olmadan).</summary>
    [Required]
    [JsonPropertyName("base64")]
    public string Base64 { get; set; } = string.Empty;
}

public class ContactSubmitRequest
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

    [MaxLength(200)]
    [JsonPropertyName("company")]
    public string? Company { get; set; }

    [Required]
    [MinLength(10)]
    [MaxLength(8000)]
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("attachments")]
    public List<ContactAttachmentSubmitDto>? Attachments { get; set; }
}
