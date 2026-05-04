using CoreService.Content.DTOs.Responses;
using CoreService.Content.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreService.Content.Controllers;

/// <summary>
/// Site içeriği (navigasyon, sayfa blokları) için API — uzaktan içerik senaryosu.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ContentController : ControllerBase
{
    private readonly ISiteContentService _siteContentService;
    private readonly ILogger<ContentController> _logger;

    public ContentController(ISiteContentService siteContentService, ILogger<ContentController> logger)
    {
        _siteContentService = siteContentService;
        _logger = logger;
    }

    /// <summary>
    /// Yerelle göre site içerik paketini getirir (GET /api/v1/content/site?locale=tr).
    /// </summary>
    [HttpGet("site")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SiteContentBundle), StatusCodes.Status200OK)]
    public async Task<ActionResult<SiteContentBundle>> GetSite(
        [FromQuery] string? locale,
        CancellationToken cancellationToken)
    {
        try
        {
            var bundle = await _siteContentService.GetBundleAsync(locale, cancellationToken);
            return Ok(bundle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Site içeriği getirilirken hata: Locale={Locale}", locale);
            return StatusCode(500, new { message = "Site içeriği getirilirken bir hata oluştu.", error = ex.Message });
        }
    }
}
