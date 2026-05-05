using CoreService.Audit.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreService.Audit.Attributes;

/// <summary>
/// Mapa Şamandıra <c>AuditActionAttribute</c> ile aynı kullanım:
/// <c>[AuditAction("TekneTipleri", "Tekne tipleri", "Tekne tipleri listelendi.")]</c>
/// Kayıt <c>http_request_logs</c> tablosuna yazılır; kullanıcı yoksa claim alanları boş kalır.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class AuditActionAttribute : ActionFilterAttribute
{
    public string IslemTuru { get; }

    public string? IslemBasligiSablon { get; }

    public string? IslemAciklamaSablon { get; }

    public AuditActionAttribute(string islemTuru, string? islemBasligiSablon = null, string? islemAciklamaSablon = null)
    {
        IslemTuru = islemTuru;
        IslemBasligiSablon = islemBasligiSablon;
        IslemAciklamaSablon = islemAciklamaSablon;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var executed = await next();
        sw.Stop();

        try
        {
            var http = context.HttpContext;
            var logger = http.RequestServices.GetRequiredService<IRequestAuditLogger>();
            await logger.LogAuditActionAsync(
                http,
                context,
                executed,
                IslemTuru,
                IslemBasligiSablon,
                IslemAciklamaSablon,
                sw.ElapsedMilliseconds,
                executed.Exception,
                http.RequestAborted);
            http.Items[AuditConstants.AuditActionLoggedKey] = true;
        }
        catch (Exception ex)
        {
            var log = context.HttpContext.RequestServices.GetService<ILogger<AuditActionAttribute>>();
            log?.LogError(ex, "AuditAction kaydı yazılamadı: {Path}", context.HttpContext.Request.Path.Value);
        }
    }
}
