using CoreService.Common;

namespace CoreService.Auth.Domain.Entities;

public class AppUser : BaseEntity
{
    public string Email { get; set; } = string.Empty;

    public string NormalizedEmail { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
}
