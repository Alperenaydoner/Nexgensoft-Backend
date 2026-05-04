using CoreService.Contact.Domain;
using CoreService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Contact.Infrastructure;

public class ContactRepository(AppDbContext db) : IContactRepository
{
    public async Task AddMessageWithAttachmentsAsync(
        ContactMessage message,
        IReadOnlyList<ContactAttachment> attachments,
        CancellationToken cancellationToken = default)
    {
        db.ContactMessages.Add(message);
        if (attachments.Count > 0)
        {
            db.ContactAttachments.AddRange(attachments);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
