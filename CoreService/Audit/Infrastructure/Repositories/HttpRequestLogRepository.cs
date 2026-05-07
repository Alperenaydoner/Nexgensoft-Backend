using CoreService.Audit.Domain.Entities;
using CoreService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Audit.Infrastructure.Repositories;

public class HttpRequestLogRepository(AppDbContext db) : IHttpRequestLogRepository
{
    public async Task AddAsync(HttpRequestLog log, CancellationToken cancellationToken = default)
    {
        db.HttpRequestLogs.Add(log);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        db.HttpRequestLogs.AsNoTracking().CountAsync(cancellationToken);

    public async Task<(IReadOnlyList<HttpRequestLog> Items, int TotalCount)> GetPagedAsync(
        int skip,
        int take,
        int? statusCode,
        string? httpMethod,
        string? pathContains,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default)
    {
        var q = db.HttpRequestLogs.AsNoTracking();
        if (statusCode is { } sc)
        {
            q = q.Where(x => x.StatusCode == sc);
        }

        if (!string.IsNullOrWhiteSpace(httpMethod))
        {
            var normalizedMethod = httpMethod.Trim().ToUpperInvariant();
            q = q.Where(x => x.HttpMethod == normalizedMethod);
        }

        if (!string.IsNullOrWhiteSpace(pathContains))
        {
            var p = pathContains.Trim();
            q = q.Where(x => x.Path.Contains(p));
        }

        if (fromUtc is { } f)
        {
            q = q.Where(x => x.OccurredAtUtc >= f);
        }

        if (toUtc is { } t)
        {
            var toUpper = NormalizeToUpperExclusive(t);
            q = q.Where(x => x.OccurredAtUtc < toUpper);
        }

        var total = await q.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await q
            .OrderByDescending(x => x.OccurredAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return (items, total);
    }

    public Task<HttpRequestLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.HttpRequestLogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    private static DateTime NormalizeToUpperExclusive(DateTime value)
    {
        return value.Second == 0 && value.Millisecond == 0
            ? value.AddMinutes(1)
            : value.AddTicks(1);
    }
}
