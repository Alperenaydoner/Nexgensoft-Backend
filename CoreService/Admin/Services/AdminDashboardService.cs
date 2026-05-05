using CoreService.Admin.DTOs;
using CoreService.Common;
using CoreService.Audit.Domain.Entities;
using CoreService.Audit.Infrastructure.Repositories;
using CoreService.Auth.Domain.Entities;
using CoreService.Auth.Infrastructure.Repositories;
using CoreService.Contact.Domain.Entities;
using CoreService.Contact.Infrastructure.Repositories;
using CoreService.Content.Infrastructure.Repositories;

namespace CoreService.Admin.Services;

public class AdminDashboardService(
    IUserRepository users,
    IContactRepository contacts,
    ISiteContentRepository siteContent,
    IHttpRequestLogRepository httpLogs) : IAdminDashboardService
{
    public async Task<AdminStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var userCount = await users.CountUsersAsync(cancellationToken).ConfigureAwait(false);
        var roleCount = await users.CountRolesAsync(cancellationToken).ConfigureAwait(false);
        var contactMessageCount = await contacts.CountMessagesAsync(cancellationToken).ConfigureAwait(false);
        var contactAttachmentCount = await contacts.CountAttachmentsAsync(cancellationToken).ConfigureAwait(false);
        var httpRequestLogCount = await httpLogs.CountAsync(cancellationToken).ConfigureAwait(false);
        var bundleCount = await siteContent.CountBundlesAsync(cancellationToken).ConfigureAwait(false);
        var stringCount = await siteContent.CountLocalizedStringsAsync(cancellationToken).ConfigureAwait(false);
        return new AdminStatsDto
        {
            UserCount = userCount,
            RoleCount = roleCount,
            ContactMessageCount = contactMessageCount,
            ContactAttachmentCount = contactAttachmentCount,
            HttpRequestLogCount = httpRequestLogCount,
            SiteContentBundleCount = bundleCount,
            SiteLocalizedStringCount = stringCount,
        };
    }

    public async Task<PagedResult<AdminUserListItemDto>> GetUsersAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var pr = new PageRequest(page, pageSize);
        var (items, total) = await users
            .GetUsersPagedWithRolesAsync(pr.Skip, pr.PageSize, cancellationToken)
            .ConfigureAwait(false);
        var dtos = items.Select(MapUserListItem).ToList();
        return PagedResult<AdminUserListItemDto>.Create(dtos, pr, total);
    }

    public async Task<AdminUserDetailDto?> GetUserDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var u = await users.GetUserByIdWithRolesAsync(id, cancellationToken).ConfigureAwait(false);
        if (u is null)
        {
            return null;
        }

        var row = MapUserListItem(u);
        return new AdminUserDetailDto
        {
            Id = row.Id,
            Email = row.Email,
            DisplayName = row.DisplayName,
            IsActive = row.IsActive,
            CreatedAtUtc = row.CreatedAtUtc,
            Roles = row.Roles,
            NormalizedEmail = u.NormalizedEmail,
        };
    }

    public async Task<PagedResult<AdminRoleListItemDto>> GetRolesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var pr = new PageRequest(page, pageSize);
        var (items, total) = await users
            .GetRolesPagedWithUserCountsAsync(pr.Skip, pr.PageSize, cancellationToken)
            .ConfigureAwait(false);
        var dtos = items.Select(r => new AdminRoleListItemDto
        {
            Id = r.Id,
            Name = r.Name,
            NormalizedName = r.NormalizedName,
            UserCount = r.UserRoles.Count,
        }).ToList();
        return PagedResult<AdminRoleListItemDto>.Create(dtos, pr, total);
    }

    public async Task<PagedResult<AdminContactMessageListItemDto>> GetContactMessagesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var pr = new PageRequest(page, pageSize);
        var (items, total) = await contacts
            .GetMessagesPagedAsync(pr.Skip, pr.PageSize, cancellationToken)
            .ConfigureAwait(false);
        var dtos = items.Select(m => new AdminContactMessageListItemDto
        {
            Id = m.Id,
            FullName = m.FullName,
            Email = m.Email,
            Company = m.Company,
            CreatedAtUtc = m.CreatedAtUtc,
            AttachmentCount = m.Attachments.Count,
        }).ToList();
        return PagedResult<AdminContactMessageListItemDto>.Create(dtos, pr, total);
    }

    public async Task<AdminContactMessageDetailDto?> GetContactMessageDetailAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var m = await contacts.GetMessageWithAttachmentsByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (m is null)
        {
            return null;
        }

        return new AdminContactMessageDetailDto
        {
            Id = m.Id,
            FullName = m.FullName,
            Email = m.Email,
            Company = m.Company,
            Message = m.Message,
            CreatedAtUtc = m.CreatedAtUtc,
            Attachments = m.Attachments.Select(MapAttachment).ToList(),
        };
    }

    public async Task<(byte[] Bytes, string ContentType, string DownloadName)?> GetContactAttachmentFileAsync(
        Guid messageId,
        Guid attachmentId,
        CancellationToken cancellationToken = default)
    {
        var att = await contacts.GetAttachmentByIdAsync(attachmentId, cancellationToken).ConfigureAwait(false);
        if (att is null || att.ContactMessageId != messageId)
        {
            return null;
        }

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(att.ContentBase64);
        }
        catch (FormatException)
        {
            return null;
        }

        return (bytes, string.IsNullOrWhiteSpace(att.ContentType) ? "application/octet-stream" : att.ContentType, att.OriginalFileName);
    }

    public async Task<PagedResult<AdminHttpRequestLogListItemDto>> GetHttpRequestLogsAsync(
        int page,
        int pageSize,
        int? statusCode,
        string? pathContains,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default)
    {
        var pr = new PageRequest(page, pageSize);
        var (items, total) = await httpLogs
            .GetPagedAsync(pr.Skip, pr.PageSize, statusCode, pathContains, fromUtc, toUtc, cancellationToken)
            .ConfigureAwait(false);
        var dtos = items.Select(MapLogListItem).ToList();
        return PagedResult<AdminHttpRequestLogListItemDto>.Create(dtos, pr, total);
    }

    public async Task<AdminHttpRequestLogDetailDto?> GetHttpRequestLogDetailAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var log = await httpLogs.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (log is null)
        {
            return null;
        }

        var row = MapLogListItem(log);
        return new AdminHttpRequestLogDetailDto
        {
            Id = row.Id,
            OccurredAtUtc = row.OccurredAtUtc,
            HttpMethod = row.HttpMethod,
            Path = row.Path,
            StatusCode = row.StatusCode,
            DurationMs = row.DurationMs,
            Success = row.Success,
            UserEmail = row.UserEmail,
            ActionType = row.ActionType,
            QueryString = log.QueryString,
            ClientIp = log.ClientIp,
            UserAgent = log.UserAgent,
            AcceptLanguage = log.AcceptLanguage,
            Referer = log.Referer,
            CorrelationId = log.CorrelationId,
            TraceId = log.TraceId,
            EnvironmentName = log.EnvironmentName,
            EndpointController = log.EndpointController,
            EndpointAction = log.EndpointAction,
            ExceptionType = log.ExceptionType,
            ExceptionMessage = log.ExceptionMessage,
            RequestBodySnippet = log.RequestBodySnippet,
            ActionTitle = log.ActionTitle,
            ActionDescription = log.ActionDescription,
            UserId = log.UserId,
            UserRoles = log.UserRoles,
        };
    }

    public async Task<AdminContentOverviewDto> GetContentOverviewAsync(CancellationToken cancellationToken = default)
    {
        var bundleCount = await siteContent.CountBundlesAsync(cancellationToken).ConfigureAwait(false);
        var totalStrings = await siteContent.CountLocalizedStringsAsync(cancellationToken).ConfigureAwait(false);
        var stringCounts = await siteContent.GetLocalizedStringCountsByLocaleAsync(cancellationToken).ConfigureAwait(false);
        var bundleLocales = await siteContent.GetBundleLocalesAsync(cancellationToken).ConfigureAwait(false);
        var localeSet = stringCounts.Select(x => x.Locale).Concat(bundleLocales).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var locales = localeSet
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Select(loc =>
            {
                var c = stringCounts.FirstOrDefault(s => string.Equals(s.Locale, loc, StringComparison.OrdinalIgnoreCase)).Count;
                var hasBundle = bundleLocales.Any(b => string.Equals(b, loc, StringComparison.OrdinalIgnoreCase));
                return new AdminContentLocaleRowDto
                {
                    Locale = loc,
                    HasBundle = hasBundle,
                    LocalizedStringCount = c,
                };
            })
            .ToList();
        return new AdminContentOverviewDto
        {
            BundleCount = bundleCount,
            TotalLocalizedStrings = totalStrings,
            Locales = locales,
        };
    }

    private static AdminUserListItemDto MapUserListItem(AppUser u) => new()
    {
        Id = u.Id,
        Email = u.Email,
        DisplayName = u.DisplayName,
        IsActive = u.IsActive,
        CreatedAtUtc = u.CreatedAtUtc,
        Roles = u.UserRoles.Select(ur => ur.Role.Name).Distinct().OrderBy(x => x).ToList(),
    };

    private static AdminContactAttachmentDto MapAttachment(ContactAttachment a) => new()
    {
        Id = a.Id,
        OriginalFileName = a.OriginalFileName,
        ContentType = a.ContentType,
        SizeBytes = a.SizeBytes,
        IsImage = a.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase),
    };

    private static AdminHttpRequestLogListItemDto MapLogListItem(HttpRequestLog log) => new()
    {
        Id = log.Id,
        OccurredAtUtc = log.OccurredAtUtc,
        HttpMethod = log.HttpMethod,
        Path = log.Path,
        StatusCode = log.StatusCode,
        DurationMs = log.DurationMs,
        Success = log.Success,
        UserEmail = log.UserEmail,
        ActionType = log.ActionType,
    };
}
