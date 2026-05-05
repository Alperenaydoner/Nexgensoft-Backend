namespace CoreService.Auth.DTOs;

/// <summary><c>Seed:AdminUser</c> bölümü — yalnızca <c>Seed:EnableAdminUserSeed</c> açıkken kullanılır.</summary>
public class AdminUserSeedOptions
{
    public const string SectionName = "Seed:AdminUser";

    public string Email { get; set; } = "admin@nexgensoft.local";

    /// <summary>İlk seed parolası; üretimde env ile verin (<c>Seed__AdminUser__Password</c>).</summary>
    public string Password { get; set; } = string.Empty;
}
