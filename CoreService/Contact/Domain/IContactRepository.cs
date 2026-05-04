namespace CoreService.Contact.Domain;

public interface IContactRepository
{
    Task AddMessageWithAttachmentsAsync(
        ContactMessage message,
        IReadOnlyList<ContactAttachment> attachments,
        CancellationToken cancellationToken = default);
}
