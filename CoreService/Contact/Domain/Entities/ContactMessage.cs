using CoreService.Common;

namespace CoreService.Contact.Domain.Entities;

public class ContactMessage : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Company { get; set; }

    public string Message { get; set; } = string.Empty;

    public ICollection<ContactAttachment> Attachments { get; set; } = new List<ContactAttachment>();
}
