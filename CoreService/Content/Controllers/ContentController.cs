using CoreService.Audit.Attributes;
using CoreService.Common;
using CoreService.Content.DTOs.Responses;
using CoreService.Content.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreService.Content.Controllers;

/// <summary>
/// Site içeriği (navigasyon, sayfa blokları) — GET <c>/api/v1/content/site</c>.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/v1/content")]
[Produces("application/json")]
public class ContentController(ISiteContentService siteContentService, ILogger<ContentController> logger) : ControllerBase
{
    /// <summary>GET /api/v1/content/site?locale=tr — site içerik paketi.</summary>
    [HttpGet("site")]
    [AuditAction("SiteIcerikPaketi", "Site içeriği", "Site içeriği getirildi. locale={locale}")]
    [ProducesResponseType(typeof(SiteContentBundle), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SiteContentBundle>> GetSiteAsync(
        [FromQuery] string? locale,
        CancellationToken cancellationToken)
    {
        try
        {
            var bundle = await siteContentService.GetBundleAsync(locale, cancellationToken);
            return Ok(bundle);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Site içeriği getirilirken hata: Locale={Locale}", locale);
            return this.ProblemFromCatalog(ProblemCatalog.Content.SiteReadFailed, ex.Message);
        }
    }
}
