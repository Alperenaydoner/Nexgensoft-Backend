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
        string? query,
        bool? hasAttachments,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default)
    {
        var q = db.ContactMessages.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var qq = query.Trim().ToUpperInvariant();
            q = q.Where(m =>
                m.FullName.ToUpper().Contains(qq) ||
                m.Email.ToUpper().Contains(qq) ||
                (m.Company != null && m.Company.ToUpper().Contains(qq)) ||
                m.Message.ToUpper().Contains(qq));
        }

        if (hasAttachments.HasValue)
        {
            q = hasAttachments.Value
                ? q.Where(m => m.Attachments.Any())
                : q.Where(m => !m.Attachments.Any());
        }

        if (fromUtc.HasValue)
        {
            q = q.Where(m => m.CreatedAtUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            q = q.Where(m => m.CreatedAtUtc <= toUtc.Value);
        }

        var asc = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
        q = (sortBy ?? string.Empty).ToLowerInvariant() switch
        {
            "fullname" => asc ? q.OrderBy(m => m.FullName) : q.OrderByDescending(m => m.FullName),
            "email" => asc ? q.OrderBy(m => m.Email) : q.OrderByDescending(m => m.Email),
            "attachments" => asc ? q.OrderBy(m => m.Attachments.Count) : q.OrderByDescending(m => m.Attachments.Count),
            _ => asc ? q.OrderBy(m => m.CreatedAtUtc) : q.OrderByDescending(m => m.CreatedAtUtc),
        };

        var total = await q.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await q
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
