using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using CoreService.Audit.Attributes;
using CoreService.Audit.Domain.Entities;
using CoreService.Audit.DTOs;
using CoreService.Audit.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace CoreService.Audit.Services;

public class RequestAuditLogger(
    IHttpRequestLogRepository repository,
    IOptions<AuditOptions> options,
    IHostEnvironment hostEnvironment,
    ILogger<RequestAuditLogger> logger) : IRequestAuditLogger
{
    public async Task LogHttpRequestAsync(
        HttpContext httpContext,
        long durationMs,
        Exception? exception,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cad = ResolveDescriptor(httpContext, null);
            var bodySnippet = await GetBodySnippetIfEnabledAsync(httpContext, cancellationToken).ConfigureAwait(false);
            var log = BuildLog(httpContext, durationMs, exception, cad, bodySnippet, actionType: null, actionTitle: null, actionDescription: null);
            await repository.AddAsync(log, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "HTTP audit kaydı yazılamadı: {Path}", httpContext.Request.Path.Value);
        }
    }

    public async Task LogAuditActionAsync(
        HttpContext httpContext,
        ActionExecutingContext executingContext,
        ActionExecutedContext executedContext,
        string actionType,
        string? actionTitleTemplate,
        string? actionDescriptionTemplate,
        long durationMs,
        Exception? exception,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = httpContext.User;
            var cad = ResolveDescriptor(httpContext, executingContext);
            var bodySnippet = await GetBodySnippetIfEnabledAsync(httpContext, cancellationToken).ConfigureAwait(false);
            var ex = executedContext.Exception ?? exception;
            var title = AuditTemplateHelper.Render(actionTitleTemplate, httpContext, executingContext, user);
            var description = AuditTemplateHelper.Render(actionDescriptionTemplate, httpContext, executingContext, user);
            var log = BuildLog(httpContext, durationMs, ex, cad, bodySnippet, actionType, title, description);
            await repository.AddAsync(log, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "AuditAction kaydı yazılamadı: {Path}", httpContext.Request.Path.Value);
        }
    }

    private static ControllerActionDescriptor? ResolveDescriptor(HttpContext http, ActionExecutingContext? executing)
    {
        if (executing?.ActionDescriptor is ControllerActionDescriptor cadFromAction)
        {
            return cadFromAction;
        }

        return http.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
    }

    private async Task<string?> GetBodySnippetIfEnabledAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        var opt = options.Value;
        if (!opt.LogRequestBody
            || httpContext.Request.ContentLength is > 512_000
            || !httpContext.Request.HasJsonContentType())
        {
            return null;
        }

        return await TryReadRequestBodySnippetAsync(httpContext, opt.MaxRequestBodyCharsToStore, cancellationToken)
            .ConfigureAwait(false);
    }

    private HttpRequestLog BuildLog(
        HttpContext httpContext,
        long durationMs,
        Exception? exception,
        ControllerActionDescriptor? cad,
        string? bodySnippet,
        string? actionType,
        string? actionTitle,
        string? actionDescription)
    {
        var user = httpContext.User;
        var status = httpContext.Response.StatusCode;
        if (exception is not null && status < 400)
        {
            status = 500;
        }

        var success = exception is null && status < 400;

        return new HttpRequestLog
        {
            Id = Guid.NewGuid(),
            OccurredAtUtc = DateTime.UtcNow,
            UserId = FirstClaim(user, ClaimTypes.NameIdentifier, "sub", "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"),
            UserEmail = FirstClaim(user, ClaimTypes.Email, "email"),
            UserRoles = RolesOrNull(user),
            HttpMethod = httpContext.Request.Method,
            Path = httpContext.Request.Path.Value ?? string.Empty,
            QueryString = httpContext.Request.QueryString.HasValue ? httpContext.Request.QueryString.Value : null,
            ClientIp = ResolveClientIp(httpContext),
            UserAgent = httpContext.Request.Headers.UserAgent.FirstOrDefault(),
            AcceptLanguage = httpContext.Request.Headers.AcceptLanguage.FirstOrDefault(),
            Referer = httpContext.Request.Headers.Referer.FirstOrDefault(),
            CorrelationId = httpContext.TraceIdentifier,
            TraceId = Activity.Current?.Id,
            EnvironmentName = hostEnvironment.EnvironmentName,
            EndpointController = cad?.ControllerName,
            EndpointAction = cad?.ActionName,
            StatusCode = status,
            DurationMs = durationMs,
            Success = success,
            ExceptionType = exception?.GetType().FullName,
            ExceptionMessage = Truncate(exception?.Message, 2000),
            RequestBodySnippet = bodySnippet,
            ActionType = actionType,
            ActionTitle = actionTitle,
            ActionDescription = actionDescription,
        };
    }

    private static string? RolesOrNull(ClaimsPrincipal user)
    {
        var roles = string.Join(',', user.FindAll(ClaimTypes.Role).Select(r => r.Value));
        return string.IsNullOrEmpty(roles) ? null : roles;
    }

    private static string? FirstClaim(ClaimsPrincipal user, params string[] types)
    {
        foreach (var t in types)
        {
            var v = user.FindFirst(t)?.Value;
            if (!string.IsNullOrWhiteSpace(v))
            {
                return v.Trim();
            }
        }

        return null;
    }

    private static string? Truncate(string? s, int max)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        return s.Length <= max ? s : s[..max] + "…";
    }

    private static string? ResolveClientIp(HttpContext context)
    {
        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwarded))
        {
            var first = forwarded.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(first))
            {
                return first;
            }
        }

        return context.Connection.RemoteIpAddress?.ToString();
    }

    private static async Task<string?> TryReadRequestBodySnippetAsync(
        HttpContext context,
        int maxChars,
        CancellationToken cancellationToken)
    {
        try
        {
            context.Request.EnableBuffering();
            context.Request.Body.Position = 0;
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
            var buffer = new char[Math.Min(maxChars + 1, 8192)];
            var sb = new StringBuilder();
            int read;
            while ((read = await reader.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false)) > 0)
            {
                sb.Append(buffer.AsSpan(0, read));
                if (sb.Length >= maxChars)
                {
                    break;
                }
            }

            context.Request.Body.Position = 0;
            var text = sb.ToString();
            if (text.Length > maxChars)
            {
                text = text[..maxChars] + "…";
            }

            return string.IsNullOrWhiteSpace(text) ? null : MaskCommonSecrets(text);
        }
        catch
        {
            try
            {
                context.Request.Body.Position = 0;
            }
            catch
            {
                // ignore
            }

            return null;
        }
    }

    private static string MaskCommonSecrets(string json)
    {
        var s = json;
        foreach (var key in new[] { "password", "access_token", "refresh_token", "client_secret", "authorization" })
        {
            s = System.Text.RegularExpressions.Regex.Replace(
                s,
                $"\"{key}\"\\s*:\\s*\"[^\"]*\"",
                $"\"{key}\":\"***\"",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant);
        }

        return s;
    }
}
