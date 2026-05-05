namespace CoreService.Common;

/// <summary>
/// Sayfalı liste cevabı — JSON: <c>items</c>, <c>pageNumber</c>, <c>pageSize</c>, <c>totalCount</c>, <c>totalPages</c>, <c>hasPreviousPage</c>, <c>hasNextPage</c> (camelCase).
/// </summary>
public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage)
{
    /// <summary><see cref="PageRequest"/> ve toplam kayıt sayısından sayfalı sonuç üretir.</summary>
    public static PagedResult<T> Create(IReadOnlyList<T> items, PageRequest request, int totalCount)
    {
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)request.PageSize);
        var hasPrev = request.PageNumber > 1 && totalCount > 0;
        var hasNext = totalPages > 0 && request.PageNumber < totalPages;
        return new PagedResult<T>(items, request.PageNumber, request.PageSize, totalCount, totalPages, hasPrev, hasNext);
    }
}
