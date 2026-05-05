using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoreService.Auth.DTOs.Requests;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(320)]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(200)]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}
