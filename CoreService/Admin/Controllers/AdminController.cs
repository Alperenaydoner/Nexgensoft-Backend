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
    public async Task<ActionResult<AdminStatsDto>> GetStatsAsync(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await dashboard.GetStatsAsync(cancellationToken));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || HttpContext.RequestAborted.IsCancellationRequested)
        {
            // Client disconnected/cancelled; avoid bubbling as unhandled 500.
            return StatusCode(499);
        }
    }

    [HttpGet("users")]
    [ProducesResponseType(typeof(PagedResult<AdminUserListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AdminUserListItemDto>>> GetUsersAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? query = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? role = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDir = null,
        CancellationToken cancellationToken = default) =>
        Ok(await dashboard.GetUsersAsync(page, pageSize, query, isActive, role, sortBy, sortDir, cancellationToken));

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
        [FromQuery] string? query = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDir = null,
        CancellationToken cancellationToken = default) =>
        Ok(await dashboard.GetRolesAsync(page, pageSize, query, sortBy, sortDir, cancellationToken));

    [HttpGet("roles/options")]
    [ProducesResponseType(typeof(AdminRoleOptionsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminRoleOptionsDto>> GetRoleOptionsAsync(CancellationToken cancellationToken) =>
        Ok(await dashboard.GetRoleOptionsAsync(cancellationToken));

    [HttpPost("users")]
    [ProducesResponseType(typeof(AdminUserDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AdminUserDetailDto>> CreateUserAsync(
        [FromBody] AdminUserUpsertRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await dashboard.CreateUserAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetUserAsync), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return this.ProblemFromCatalog(ProblemCatalog.Admin.UsersCreateValidation, ex.Message);
        }
    }

    [HttpPut("users/{id:guid}")]
    [ProducesResponseType(typeof(AdminUserDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminUserDetailDto>> UpdateUserAsync(
        Guid id,
        [FromBody] AdminUserUpsertRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await dashboard.UpdateUserAsync(id, request, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return this.ProblemFromCatalog(ProblemCatalog.Admin.UsersUpdateValidation, ex.Message);
        }
    }

    [HttpDelete("users/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await dashboard.DeleteUserAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("users/bulk-delete")]
    [ProducesResponseType(typeof(AdminBulkDeleteResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminBulkDeleteResultDto>> BulkDeleteUsersAsync(
        [FromBody] AdminBulkDeleteUsersRequestDto request,
        CancellationToken cancellationToken)
    {
        var deletedCount = await dashboard.DeleteUsersAsync(request.Ids, cancellationToken);
        return Ok(new AdminBulkDeleteResultDto { DeletedCount = deletedCount });
    }

    [HttpPost("roles")]
    [ProducesResponseType(typeof(AdminRoleListItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AdminRoleListItemDto>> CreateRoleAsync(
        [FromBody] AdminRoleUpsertRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await dashboard.CreateRoleAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, created);
        }
        catch (InvalidOperationException ex)
        {
            return this.ProblemFromCatalog(ProblemCatalog.Admin.RolesCreateValidation, ex.Message);
        }
    }

    [HttpPut("roles/{id:guid}")]
    [ProducesResponseType(typeof(AdminRoleListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminRoleListItemDto>> UpdateRoleAsync(
        Guid id,
        [FromBody] AdminRoleUpsertRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await dashboard.UpdateRoleAsync(id, request, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return this.ProblemFromCatalog(ProblemCatalog.Admin.RolesUpdateValidation, ex.Message);
        }
    }

    [HttpDelete("roles/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRoleAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await dashboard.DeleteRoleAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("contact/messages")]
    [ProducesResponseType(typeof(PagedResult<AdminContactMessageListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AdminContactMessageListItemDto>>> GetContactMessagesAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? query = null,
        [FromQuery] bool? hasAttachments = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDir = null,
        CancellationToken cancellationToken = default) =>
        Ok(await dashboard.GetContactMessagesAsync(page, pageSize, query, hasAttachments, fromUtc, toUtc, sortBy, sortDir, cancellationToken));

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

    [HttpGet("applications")]
    [ProducesResponseType(typeof(PagedResult<AdminJobApplicationListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AdminJobApplicationListItemDto>>> GetJobApplicationsAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? query = null,
        [FromQuery] string? position = null,
        [FromQuery] bool? hasAttachments = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDir = null,
        CancellationToken cancellationToken = default) =>
        Ok(await dashboard.GetJobApplicationsAsync(page, pageSize, query, position, hasAttachments, fromUtc, toUtc, sortBy, sortDir, cancellationToken));

    [HttpGet("applications/{id:guid}")]
    [ProducesResponseType(typeof(AdminJobApplicationDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminJobApplicationDetailDto>> GetJobApplicationAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var a = await dashboard.GetJobApplicationDetailAsync(id, cancellationToken);
        return a is null ? NotFound() : Ok(a);
    }

    [HttpGet("applications/{applicationId:guid}/attachments/{attachmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobApplicationAttachmentAsync(
        Guid applicationId,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var file = await dashboard.GetJobApplicationAttachmentFileAsync(applicationId, attachmentId, cancellationToken);
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

    [HttpGet("content/locales/{locale}")]
    [ProducesResponseType(typeof(AdminContentLocaleDetailDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminContentLocaleDetailDto>> GetContentLocaleAsync(
        string locale,
        CancellationToken cancellationToken) =>
        Ok(await dashboard.GetContentLocaleDetailAsync(locale, cancellationToken));

    [HttpPost("content/locales/save")]
    [ProducesResponseType(typeof(AdminContentLocaleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AdminContentLocaleDetailDto>> SaveContentLocaleAsync(
        [FromBody] AdminContentBulkUpsertRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await dashboard.UpsertContentLocaleAsync(request, cancellationToken));
        }
        catch (InvalidOperationException ex)
        {
            return this.ProblemFromCatalog(ProblemCatalog.Admin.ContentSaveValidation, ex.Message);
        }
    }

    [HttpGet("content/locales/{locale}/audit")]
    [ProducesResponseType(typeof(IReadOnlyList<AdminContentAuditRowDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AdminContentAuditRowDto>>> GetContentLocaleAuditAsync(
        string locale,
        [FromQuery] int take = 20,
        CancellationToken cancellationToken = default) =>
        Ok(await dashboard.GetRecentContentAuditAsync(locale, take, cancellationToken));
}
