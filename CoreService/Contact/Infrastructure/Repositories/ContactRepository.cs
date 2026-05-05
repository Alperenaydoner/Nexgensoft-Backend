using CoreService.Contact.Domain.Entities;
using CoreService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Contact.Infrastructure.Repositories;

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

    public Task<int> CountMessagesAsync(CancellationToken cancellationToken = default) =>
        db.ContactMessages.AsNoTracking().CountAsync(cancellationToken);

    public Task<int> CountAttachmentsAsync(CancellationToken cancellationToken = default) =>
        db.ContactAttachments.AsNoTracking().CountAsync(cancellationToken);

    public async Task<(IReadOnlyList<ContactMessage> Items, int TotalCount)> GetMessagesPagedAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var q = db.ContactMessages.AsNoTracking();
        var total = await q.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await q
            .OrderByDescending(m => m.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .Include(m => m.Attachments)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return (items, total);
    }

    public Task<ContactMessage?> GetMessageWithAttachmentsByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.ContactMessages.AsNoTracking()
            .Include(m => m.Attachments)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public Task<ContactAttachment?> GetAttachmentByIdAsync(Guid attachmentId, CancellationToken cancellationToken = default) =>
        db.ContactAttachments.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == attachmentId, cancellationToken);
}
