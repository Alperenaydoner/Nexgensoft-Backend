using CoreService.Common;

namespace CoreService.Auth.Domain.Entities;

/// <summary>Bileşik anahtar (UserId, RoleId); ayrı audit <see cref="UserId"/> sütunu yok (çakışma).</summary>
public class AppUserRole : ISoftDeletable, IAuditableTimestamps
{
    public Guid UserId { get; set; }

    public AppUser User { get; set; } = null!;

    public Guid RoleId { get; set; }

    public AppRole Role { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }
}
