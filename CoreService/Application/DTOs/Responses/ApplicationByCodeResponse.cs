using System.Text.Json.Serialization;

namespace CoreService.Application.DTOs.Responses;

public class ApplicationByCodeResponse
{
    [JsonPropertyName("applicationCode")]
    public Guid ApplicationCode { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;

    [JsonPropertyName("coverLetter")]
    public string? CoverLetter { get; set; }
}
