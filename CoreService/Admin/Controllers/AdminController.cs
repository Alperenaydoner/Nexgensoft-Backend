using CoreService.Admin.DTOs;
using CoreService.Admin.Services;
using CoreService.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreService.Admin.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class AdminController(IAdminDashboardService dashboard) : ControllerBase
{
    [HttpGet("ping")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Ping() => Ok(new { ok = true, area = "admin" });

    [HttpGet("stats")]
    [ProducesResponseType(typeof(AdminStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminStatsDto>> GetStatsAsync(CancellationToken cancellationToken) =>
        Ok(await dashboard.GetStatsAsync(cancellationToken));

    [HttpGet("users")]
    [ProducesResponseType(typeof(PagedResult<AdminUserListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AdminUserListItemDto>>> GetUsersAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default) =>
        Ok(await dashboard.GetUsersAsync(page, pageSize, cancellationToken));

    [HttpGet("users/{id:guid}")]
    [ProducesResponseType(typeof(AdminUserDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminUserDetailDto>> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var u = await dashboard.GetUserDetailAsync(id, cancellationToken);
        return u is null ? NotFound() : Ok(u);
    }

    [HttpGet("roles")]
    [ProducesResponseType(typeof(PagedResult<AdminRoleListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AdminRoleListItemDto>>> GetRolesAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default) =>
        Ok(await dashboard.GetRolesAsync(page, pageSize, cancellationToken));

    [HttpGet("contact/messages")]
    [ProducesResponseType(typeof(PagedResult<AdminContactMessageListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AdminContactMessageListItemDto>>> GetContactMessagesAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default) =>
        Ok(await dashboard.GetContactMessagesAsync(page, pageSize, cancellationToken));

    [HttpGet("contact/messages/{id:guid}")]
    [ProducesResponseType(typeof(AdminContactMessageDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminContactMessageDetailDto>> GetContactMessageAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var m = await dashboard.GetContactMessageDetailAsync(id, cancellationToken);
        return m is null ? NotFound() : Ok(m);
    }

    [HttpGet("contact/messages/{messageId:guid}/attachments/{attachmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContactAttachmentAsync(
        Guid messageId,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var file = await dashboard.GetContactAttachmentFileAsync(messageId, attachmentId, cancellationToken);
        if (file is null)
        {
            return NotFound();
        }

        var (bytes, contentType, downloadName) = file.Value;
        var inline = contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        return File(bytes, contentType, downloadName, inline);
    }

    [HttpGet("audit/logs")]
    [ProducesResponseType(typeof(PagedResult<AdminHttpRequestLogListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AdminHttpRequestLogListItemDto>>> GetLogsAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] int? statusCode = null,
        [FromQuery] string? pathContains = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        CancellationToken cancellationToken = default) =>
        Ok(await dashboard.GetHttpRequestLogsAsync(page, pageSize, statusCode, pathContains, fromUtc, toUtc, cancellationToken));

    [HttpGet("audit/logs/{id:guid}")]
    [ProducesResponseType(typeof(AdminHttpRequestLogDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminHttpRequestLogDetailDto>> GetLogAsync(Guid id, CancellationToken cancellationToken)
    {
        var log = await dashboard.GetHttpRequestLogDetailAsync(id, cancellationToken);
        return log is null ? NotFound() : Ok(log);
    }

    [HttpGet("content/overview")]
    [ProducesResponseType(typeof(AdminContentOverviewDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminContentOverviewDto>> GetContentOverviewAsync(CancellationToken cancellationToken) =>
        Ok(await dashboard.GetContentOverviewAsync(cancellationToken));
}
