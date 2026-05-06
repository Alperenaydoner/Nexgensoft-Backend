namespace CoreService.Common;

public interface ISoftDeletable
{
    bool IsActive { get; set; }

    bool IsDeleted { get; set; }
}

public interface IAuditableTimestamps
{
    DateTime CreatedAtUtc { get; set; }

    DateTime? UpdatedAtUtc { get; set; }
}

/// <summary>
/// Ortak audit + soft-delete alanları (kimlik <typeparamref name="TKey"/>).
/// </summary>
public abstract class AuditableEntityBase : ISoftDeletable, IAuditableTimestamps
{
    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>Dahili kullanıcı (<c>app_users</c>) ile isteğe bağlı ilişki; oluşturan / son işlem yapan.</summary>
    public Guid? UserId { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }
}

public abstract class BaseEntity<TKey> : AuditableEntityBase where TKey : notnull
{
    public TKey Id { get; set; } = default!;
}

public abstract class BaseEntity : BaseEntity<Guid> { }
