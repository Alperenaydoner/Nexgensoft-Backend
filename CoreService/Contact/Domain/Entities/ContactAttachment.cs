using CoreService.Common;

namespace CoreService.Contact.Domain.Entities;

public class ContactAttachment : BaseEntity
{
    public Guid ContactMessageId { get; set; }

    public ContactMessage ContactMessage { get; set; } = null!;

    public string OriginalFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    /// <summary>İçerik Base64 (UTF-16 nvarchar; büyük dosyalar için dikkat).</summary>
    public string ContentBase64 { get; set; } = string.Empty;
}
