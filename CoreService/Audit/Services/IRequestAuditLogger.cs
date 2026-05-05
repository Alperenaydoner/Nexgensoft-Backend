using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreService.Audit.Services;

public interface IRequestAuditLogger
{
    Task LogHttpRequestAsync(
        HttpContext httpContext,
        long durationMs,
        Exception? exception,
        CancellationToken cancellationToken = default);

    /// <summary>Mapa <c>AuditActionAttribute</c> kaydı — HTTP satırı + işlem alanları.</summary>
    Task LogAuditActionAsync(
        HttpContext httpContext,
        ActionExecutingContext executingContext,
        ActionExecutedContext executedContext,
        string actionType,
        string? actionTitleTemplate,
        string? actionDescriptionTemplate,
        long durationMs,
        Exception? exception,
        CancellationToken cancellationToken = default);
}
