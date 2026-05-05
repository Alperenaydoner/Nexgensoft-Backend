namespace CoreService.Contact.Domain.Entities;

public class ContactMessage
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Company { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public ICollection<ContactAttachment> Attachments { get; set; } = new List<ContactAttachment>();
}
