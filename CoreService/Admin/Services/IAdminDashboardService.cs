using CoreService.Admin.DTOs;
using CoreService.Common;

namespace CoreService.Admin.Services;

public interface IAdminDashboardService
{
    Task<AdminStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<AdminUserListItemDto>> GetUsersAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<AdminUserDetailDto?> GetUserDetailAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<AdminRoleListItemDto>> GetRolesAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<PagedResult<AdminContactMessageListItemDto>> GetContactMessagesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<AdminContactMessageDetailDto?> GetContactMessageDetailAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(byte[] Bytes, string ContentType, string DownloadName)?> GetContactAttachmentFileAsync(
        Guid messageId,
        Guid attachmentId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AdminHttpRequestLogListItemDto>> GetHttpRequestLogsAsync(
        int page,
        int pageSize,
        int? statusCode,
        string? pathContains,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default);

    Task<AdminHttpRequestLogDetailDto?> GetHttpRequestLogDetailAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AdminContentOverviewDto> GetContentOverviewAsync(CancellationToken cancellationToken = default);
}
