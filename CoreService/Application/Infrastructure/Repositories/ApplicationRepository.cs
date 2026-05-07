using CoreService.Application.Domain.Entities;
using CoreService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Application.Infrastructure.Repositories;

public class ApplicationRepository(AppDbContext db) : IApplicationRepository
{
    public Task AddApplicationWithAttachmentsAsync(
        JobApplication application,
        IReadOnlyList<JobApplicationAttachment> attachments,
        CancellationToken cancellationToken = default)
    {
        db.JobApplications.Add(application);
        if (attachments.Count > 0)
        {
            db.JobApplicationAttachments.AddRange(attachments);
        }

        return Task.CompletedTask;
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
            var toUpper = NormalizeToUpperExclusive(toUtc.Value);
            q = q.Where(a => a.CreatedAtUtc < toUpper);
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
        var pageRows = await q
            .Skip(skip)
            .Take(take)
            .Select(a => new
            {
                a.Id,
                a.FullName,
                a.Email,
                a.Phone,
                a.Position,
                a.CreatedAtUtc,
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (pageRows.Count == 0)
        {
            return ([], total);
        }

        var pageIds = pageRows.Select(x => x.Id).ToList();
        var attachmentCounts = await db.JobApplicationAttachments
            .AsNoTracking()
            .Where(a => pageIds.Contains(a.JobApplicationId))
            .GroupBy(a => a.JobApplicationId)
            .Select(g => new { JobApplicationId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.JobApplicationId, x => x.Count, cancellationToken)
            .ConfigureAwait(false);

        var items = pageRows
            .Select(x => new JobApplicationListRow(
                x.Id,
                x.FullName,
                x.Email,
                x.Phone,
                x.Position,
                x.CreatedAtUtc,
                attachmentCounts.GetValueOrDefault(x.Id, 0)))
            .ToList();
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

    public void AddAttachments(IReadOnlyList<JobApplicationAttachment> attachments)
    {
        if (attachments.Count == 0)
        {
            return;
        }

        db.JobApplicationAttachments.AddRange(attachments);
    }

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

    private static DateTime NormalizeToUpperExclusive(DateTime value)
    {
        // datetime-local inputs are minute precision; include full selected minute.
        return value.Second == 0 && value.Millisecond == 0
            ? value.AddMinutes(1)
            : value.AddTicks(1);
    }
}
