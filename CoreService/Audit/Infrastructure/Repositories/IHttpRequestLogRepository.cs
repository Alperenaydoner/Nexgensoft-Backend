using CoreService.Audit.Domain.Entities;

namespace CoreService.Audit.Infrastructure.Repositories;

public interface IHttpRequestLogRepository
{
    Task AddAsync(HttpRequestLog log, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<HttpRequestLog> Items, int TotalCount)> GetPagedAsync(
        int skip,
        int take,
        int? statusCode,
        string? httpMethod,
        string? pathContains,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default);

    Task<HttpRequestLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
