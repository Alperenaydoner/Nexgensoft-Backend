using CoreService.Application.Domain.Entities;
using CoreService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Application.Infrastructure.Repositories;

public class ApplicationRepository(AppDbContext db) : IApplicationRepository
{
    public async Task AddApplicationWithAttachmentsAsync(
        JobApplication application,
        IReadOnlyList<JobApplicationAttachment> attachments,
        CancellationToken cancellationToken = default)
    {
        db.JobApplications.Add(application);
        if (attachments.Count > 0)
        {
            db.JobApplicationAttachments.AddRange(attachments);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<int> CountApplicationsAsync(CancellationToken cancellationToken = default) =>
        db.JobApplications.AsNoTracking().CountAsync(cancellationToken);

    public Task<int> CountAttachmentsAsync(CancellationToken cancellationToken = default) =>
        db.JobApplicationAttachments.AsNoTracking().CountAsync(cancellationToken);

    public async Task<(IReadOnlyList<JobApplicationListRow> Items, int TotalCount)> GetApplicationsPagedAsync(
        int skip,
        int take,
        string? query,
        string? position,
        bool? hasAttachments,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default)
    {
        var q = db.JobApplications.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var qq = query.Trim().ToUpperInvariant();
            q = q.Where(a =>
                a.FullName.ToUpper().Contains(qq) ||
                a.Email.ToUpper().Contains(qq) ||
                (a.Phone != null && a.Phone.ToUpper().Contains(qq)) ||
                a.Position.ToUpper().Contains(qq) ||
                (a.CoverLetter != null && a.CoverLetter.ToUpper().Contains(qq)));
        }

        if (!string.IsNullOrWhiteSpace(position))
        {
            var pp = position.Trim().ToUpperInvariant();
            q = q.Where(a => a.Position.ToUpper() == pp);
        }

        if (hasAttachments.HasValue)
        {
            q = hasAttachments.Value
                ? q.Where(a => a.Attachments.Any())
                : q.Where(a => !a.Attachments.Any());
        }

        if (fromUtc.HasValue)
        {
            q = q.Where(a => a.CreatedAtUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            q = q.Where(a => a.CreatedAtUtc <= toUtc.Value);
        }

        var asc = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
        q = (sortBy ?? string.Empty).ToLowerInvariant() switch
        {
            "fullname" => asc ? q.OrderBy(a => a.FullName) : q.OrderByDescending(a => a.FullName),
            "email" => asc ? q.OrderBy(a => a.Email) : q.OrderByDescending(a => a.Email),
            "position" => asc ? q.OrderBy(a => a.Position) : q.OrderByDescending(a => a.Position),
            "attachments" => asc ? q.OrderBy(a => a.Attachments.Count) : q.OrderByDescending(a => a.Attachments.Count),
            _ => asc ? q.OrderBy(a => a.CreatedAtUtc) : q.OrderByDescending(a => a.CreatedAtUtc),
        };

        var total = await q.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await q
            .Skip(skip)
            .Take(take)
            .Select(a => new JobApplicationListRow(
                a.Id,
                a.FullName,
                a.Email,
                a.Phone,
                a.Position,
                a.CreatedAtUtc,
                a.Attachments.Count))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return (items, total);
    }

    public Task<JobApplication?> GetApplicationWithAttachmentsByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.JobApplications.AsNoTracking()
            .Include(a => a.Attachments)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<JobApplication?> GetApplicationByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.JobApplications
            .Include(a => a.Attachments)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);

    public Task<JobApplicationAttachment?> GetAttachmentByIdAsync(Guid attachmentId, CancellationToken cancellationToken = default) =>
        db.JobApplicationAttachments.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == attachmentId, cancellationToken);

    public async Task<IReadOnlyList<string>> GetActivePositionNamesAsync(CancellationToken cancellationToken = default) =>
        await db.JobPositions
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);
}
