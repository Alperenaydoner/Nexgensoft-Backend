using CoreService.Common;

namespace CoreService.Auth.Domain.Entities;

public class AppRole : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
}
