using System.Text.Json.Serialization;

namespace CoreService.Auth.DTOs.Responses;

public class LoginResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("expiresAtUtc")]
    public DateTime ExpiresAtUtc { get; set; }

    [JsonPropertyName("user")]
    public CurrentUserDto User { get; set; } = null!;
}

public class CurrentUserDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("roles")]
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}
