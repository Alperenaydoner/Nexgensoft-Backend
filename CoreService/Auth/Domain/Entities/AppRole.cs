namespace CoreService.Auth.Domain.Entities;

public class AppRole
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
}
