using CoreService.Admin.DTOs;
using CoreService.Common;

namespace CoreService.Admin.Services;

public interface IAdminDashboardService
{
    Task<AdminStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<AdminUserListItemDto>> GetUsersAsync(
        int page,
        int pageSize,
        string? query,
        bool? isActive,
        string? role,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default);

    Task<AdminUserDetailDto?> GetUserDetailAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<AdminRoleListItemDto>> GetRolesAsync(
        int page,
        int pageSize,
        string? query,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default);

    Task<AdminRoleOptionsDto> GetRoleOptionsAsync(CancellationToken cancellationToken = default);

    Task<AdminUserDetailDto> CreateUserAsync(AdminUserUpsertRequestDto request, CancellationToken cancellationToken = default);

    Task<AdminUserDetailDto?> UpdateUserAsync(Guid id, AdminUserUpsertRequestDto request, CancellationToken cancellationToken = default);

    Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> DeleteUsersAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default);

    Task<AdminRoleListItemDto> CreateRoleAsync(AdminRoleUpsertRequestDto request, CancellationToken cancellationToken = default);

    Task<AdminRoleListItemDto?> UpdateRoleAsync(Guid id, AdminRoleUpsertRequestDto request, CancellationToken cancellationToken = default);

    Task<bool> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<AdminContactMessageListItemDto>> GetContactMessagesAsync(
        int page,
        int pageSize,
        string? query,
        bool? hasAttachments,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default);

    Task<AdminContactMessageDetailDto?> GetContactMessageDetailAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(byte[] Bytes, string ContentType, string DownloadName)?> GetContactAttachmentFileAsync(
        Guid messageId,
        Guid attachmentId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AdminJobApplicationListItemDto>> GetJobApplicationsAsync(
        int page,
        int pageSize,
        string? query,
        string? position,
        bool? hasAttachments,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default);

    Task<AdminJobApplicationDetailDto?> GetJobApplicationDetailAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(byte[] Bytes, string ContentType, string DownloadName)?> GetJobApplicationAttachmentFileAsync(
        Guid applicationId,
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

    Task<AdminContentLocaleDetailDto> GetContentLocaleDetailAsync(string locale, CancellationToken cancellationToken = default);

    Task<AdminContentLocaleDetailDto> UpsertContentLocaleAsync(
        AdminContentBulkUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AdminContentAuditRowDto>> GetRecentContentAuditAsync(
        string locale,
        int take,
        CancellationToken cancellationToken = default);
}
