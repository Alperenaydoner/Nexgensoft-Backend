using CoreService.Contact.Domain.Entities;

namespace CoreService.Contact.Infrastructure.Repositories;

public interface IContactRepository
{
    Task AddMessageWithAttachmentsAsync(
        ContactMessage message,
        IReadOnlyList<ContactAttachment> attachments,
        CancellationToken cancellationToken = default);

    Task<int> CountMessagesAsync(CancellationToken cancellationToken = default);

    Task<int> CountAttachmentsAsync(CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ContactMessage> Items, int TotalCount)> GetMessagesPagedAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<ContactMessage?> GetMessageWithAttachmentsByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ContactAttachment?> GetAttachmentByIdAsync(Guid attachmentId, CancellationToken cancellationToken = default);
}
