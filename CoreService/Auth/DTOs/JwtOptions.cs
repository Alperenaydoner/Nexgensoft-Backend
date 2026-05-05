using System.ComponentModel.DataAnnotations;

namespace CoreService.Auth.DTOs;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; set; } = "Nexgensoft";

    [Required]
    public string Audience { get; set; } = "Nexgensoft";

    /// <summary>HS256 için en az 32 karakter (üretimde env ile verin).</summary>
    [Required]
    [MinLength(32)]
    public string SigningKey { get; set; } = string.Empty;

    [Range(5, 10080)]
    public int AccessTokenMinutes { get; set; } = 480;
}
