using System.Diagnostics;
using CoreService.Audit;
using CoreService.Audit.DTOs;
using Microsoft.AspNetCore.Http;
using CoreService.Audit.Services;
using Microsoft.Extensions.Options;

namespace CoreService.Audit.Infrastructure;

public class RequestAuditMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory, IOptions<AuditOptions> options)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var opt = options.Value;
        if (!opt.Enabled || IsExcluded(context.Request.Path, opt.ExcludedPathPrefixes))
        {
            await next(context);
            return;
        }

        var sw = Stopwatch.StartNew();
        Exception? caught = null;
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            caught = ex;
            throw;
        }
        finally
        {
            sw.Stop();
            try
            {
                var skipMiddlewareLog = context.Items.TryGetValue(AuditConstants.AuditActionLoggedKey, out var auditActionDone)
                    && auditActionDone is true;
                if (!skipMiddlewareLog)
                {
                    await using var scope = scopeFactory.CreateAsyncScope();
                    var writer = scope.ServiceProvider.GetRequiredService<IRequestAuditLogger>();
                    await writer.LogHttpRequestAsync(context, sw.ElapsedMilliseconds, caught, context.RequestAborted);
                }
            }
            catch
            {
                // Audit asla isteği patlatmaz
            }
        }
    }

    private static bool IsExcluded(PathString path, string[] prefixes)
    {
        if (prefixes.Length == 0)
        {
            return false;
        }

        var p = path.Value ?? string.Empty;
        foreach (var prefix in prefixes)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                continue;
            }

            var normalized = prefix.StartsWith('/') ? prefix : "/" + prefix;
            if (p.StartsWith(normalized, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
