using CoreService.Admin.DTOs;
using CoreService.Application.Domain.Entities;
using CoreService.Application.Infrastructure.Repositories;
using CoreService.Common;
using CoreService.Common.Localization;
using CoreService.Audit.Domain.Entities;
using CoreService.Audit.Infrastructure.Repositories;
using CoreService.Auth.Domain.Entities;
using CoreService.Auth.Infrastructure.Repositories;
using CoreService.Contact.Domain.Entities;
using CoreService.Contact.Infrastructure.Repositories;
using CoreService.Content.Infrastructure.Repositories;
using CoreService.Content.Domain.Entities;

namespace CoreService.Admin.Services;

public class AdminDashboardService(
    IUserRepository users,
    IContactRepository contacts,
    IApplicationRepository applications,
    ISiteContentRepository siteContent,
    IHttpRequestLogRepository httpLogs,
    IApiTextLocalizer loc) : IAdminDashboardService
{
    public async Task<AdminStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        // Repositories share the same scoped DbContext; keep these awaited sequentially
        // to avoid "A second operation was started on this context instance" errors.
        var userCount = await users.CountUsersAsync(cancellationToken).ConfigureAwait(false);
        var roleCount = await users.CountRolesAsync(cancellationToken).ConfigureAwait(false);
        var contactMessageCount = await contacts.CountMessagesAsync(cancellationToken).ConfigureAwait(false);
        var contactAttachmentCount = await contacts.CountAttachmentsAsync(cancellationToken).ConfigureAwait(false);
        var jobApplicationCount = await applications.CountApplicationsAsync(cancellationToken).ConfigureAwait(false);
        var jobApplicationAttachmentCount = await applications.CountAttachmentsAsync(cancellationToken).ConfigureAwait(false);
        var httpRequestLogCount = await httpLogs.CountAsync(cancellationToken).ConfigureAwait(false);
        var bundleCount = await siteContent.CountBundlesAsync(cancellationToken).ConfigureAwait(false);
        var stringCount = await siteContent.CountLocalizedStringsAsync(cancellationToken).ConfigureAwait(false);

        return new AdminStatsDto
        {
            UserCount = userCount,
            RoleCount = roleCount,
            ContactMessageCount = contactMessageCount,
            ContactAttachmentCount = contactAttachmentCount,
            JobApplicationCount = jobApplicationCount,
            JobApplicationAttachmentCount = jobApplicationAttachmentCount,
            HttpRequestLogCount = httpRequestLogCount,
            SiteContentBundleCount = bundleCount,
            SiteLocalizedStringCount = stringCount,
        };
    }

    public async Task<PagedResult<AdminUserListItemDto>> GetUsersAsync(
        int page,
        int pageSize,
        string? query,
        bool? isActive,
        string? role,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default)
    {
        var pr = new PageRequest(page, pageSize);
        var (items, total) = await users
            .GetUsersPagedWithRolesAsync(pr.Skip, pr.PageSize, query, isActive, role, sortBy, sortDir, cancellationToken)
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
        string? query,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default)
    {
        var pr = new PageRequest(page, pageSize);
        var (items, total) = await users
            .GetRolesPagedWithUserCountsAsync(pr.Skip, pr.PageSize, query, sortBy, sortDir, cancellationToken)
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

    public async Task<AdminRoleOptionsDto> GetRoleOptionsAsync(CancellationToken cancellationToken = default)
    {
        var items = await users.GetAllRoleNamesAsync(cancellationToken).ConfigureAwait(false);
        return new AdminRoleOptionsDto { Items = items };
    }

    public async Task<OperationResult<AdminUserDetailDto>> CreateUserAsync(AdminUserUpsertRequestDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var email = request.Email?.Trim() ?? string.Empty;
        var normalizedEmail = email.ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(email))
        {
            return OperationResult<AdminUserDetailDto>.Validation(CreateValidation("email", "Validation.Admin.Users.EmailRequired"));
        }

        if (await users.AnyUserWithNormalizedEmailAsync(normalizedEmail, cancellationToken).ConfigureAwait(false))
        {
            return OperationResult<AdminUserDetailDto>.Validation(CreateValidation("email", "Validation.Admin.Users.EmailAlreadyExists"));
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return OperationResult<AdminUserDetailDto>.Validation(CreateValidation("password", "Validation.Admin.Users.PasswordRequired"));
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            NormalizedEmail = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? email : request.DisplayName.Trim(),
            IsActive = request.IsActive,
            CreatedAtUtc = DateTime.UtcNow,
        };
        var created = await users.CreateUserAsync(user, request.Roles ?? Array.Empty<string>(), cancellationToken).ConfigureAwait(false);
        var dto = await BuildUserDetailDtoAsync(created.Id, cancellationToken).ConfigureAwait(false);
        return OperationResult<AdminUserDetailDto>.Ok(dto);
    }

    public async Task<OperationResult<AdminUserDetailDto?>> UpdateUserAsync(Guid id, AdminUserUpsertRequestDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var email = request.Email?.Trim() ?? string.Empty;
        var normalizedEmail = email.ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(email))
        {
            return OperationResult<AdminUserDetailDto?>.Validation(CreateValidation("email", "Validation.Admin.Users.EmailRequired"));
        }

        var existingByEmail = await users.GetByNormalizedEmailWithRolesAsync(normalizedEmail, cancellationToken).ConfigureAwait(false);
        if (existingByEmail is not null && existingByEmail.Id != id)
        {
            return OperationResult<AdminUserDetailDto?>.Validation(CreateValidation("email", "Validation.Admin.Users.EmailAlreadyExists"));
        }

        var updated = await users.UpdateUserAsync(
            id,
            email,
            string.IsNullOrWhiteSpace(request.DisplayName) ? email : request.DisplayName.Trim(),
            request.IsActive,
            request.Password,
            request.Roles ?? Array.Empty<string>(),
            cancellationToken).ConfigureAwait(false);
        if (updated is null)
        {
            return OperationResult<AdminUserDetailDto?>.Ok(null);
        }

        var dto = await BuildUserDetailDtoAsync(updated.Id, cancellationToken).ConfigureAwait(false);
        return OperationResult<AdminUserDetailDto?>.Ok(dto);
    }

    public Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default) =>
        users.DeleteUserAsync(id, cancellationToken);

    public Task<int> DeleteUsersAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default) =>
        users.DeleteUsersAsync(ids, cancellationToken);

    public async Task<OperationResult<AdminRoleListItemDto>> CreateRoleAsync(AdminRoleUpsertRequestDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            return OperationResult<AdminRoleListItemDto>.Validation(CreateValidation("name", "Validation.Admin.Roles.NameRequired"));
        }

        var normalizedName = name.ToUpperInvariant();
        if (await users.AnyRoleWithNormalizedNameAsync(normalizedName, cancellationToken).ConfigureAwait(false))
        {
            return OperationResult<AdminRoleListItemDto>.Validation(CreateValidation("name", "Validation.Admin.Roles.AlreadyExists"));
        }

        var role = await users.CreateRoleAsync(new AppRole
        {
            Id = Guid.NewGuid(),
            Name = name,
            NormalizedName = normalizedName,
        }, cancellationToken).ConfigureAwait(false);
        return OperationResult<AdminRoleListItemDto>.Ok(
            new AdminRoleListItemDto { Id = role.Id, Name = role.Name, NormalizedName = role.NormalizedName, UserCount = 0 });
    }

    public async Task<OperationResult<AdminRoleListItemDto?>> UpdateRoleAsync(Guid id, AdminRoleUpsertRequestDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            return OperationResult<AdminRoleListItemDto?>.Validation(CreateValidation("name", "Validation.Admin.Roles.NameRequired"));
        }

        var normalizedName = name.ToUpperInvariant();
        var existing = await users.GetRoleByNormalizedNameAsync(normalizedName, cancellationToken).ConfigureAwait(false);
        if (existing is not null && existing.Id != id)
        {
            return OperationResult<AdminRoleListItemDto?>.Validation(CreateValidation("name", "Validation.Admin.Roles.AlreadyExists"));
        }

        var role = await users.UpdateRoleAsync(id, name, cancellationToken).ConfigureAwait(false);
        if (role is null)
        {
            return OperationResult<AdminRoleListItemDto?>.Ok(null);
        }

        return OperationResult<AdminRoleListItemDto?>.Ok(new AdminRoleListItemDto
        {
            Id = role.Id,
            Name = role.Name,
            NormalizedName = role.NormalizedName,
            UserCount = role.UserRoles.Count,
        });
    }

    public Task<bool> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default) =>
        users.DeleteRoleAsync(id, cancellationToken);

    public async Task<PagedResult<AdminContactMessageListItemDto>> GetContactMessagesAsync(
        int page,
        int pageSize,
        string? query,
        bool? hasAttachments,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default)
    {
        var pr = new PageRequest(page, pageSize);
        var (items, total) = await contacts
            .GetMessagesPagedAsync(pr.Skip, pr.PageSize, query, hasAttachments, fromUtc, toUtc, sortBy, sortDir, cancellationToken)
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

    public async Task<AdminAttachmentFileDto?> GetContactAttachmentFileAsync(
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

        return new AdminAttachmentFileDto(
            bytes,
            string.IsNullOrWhiteSpace(att.ContentType) ? "application/octet-stream" : att.ContentType,
            att.OriginalFileName);
    }

    public async Task<PagedResult<AdminJobApplicationListItemDto>> GetJobApplicationsAsync(
        int page,
        int pageSize,
        string? query,
        string? position,
        bool? hasAttachments,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default)
    {
        var pr = new PageRequest(page, pageSize);
        var (items, total) = await applications
            .GetApplicationsPagedAsync(pr.Skip, pr.PageSize, query, position, hasAttachments, fromUtc, toUtc, sortBy, sortDir, cancellationToken)
            .ConfigureAwait(false);
        var dtos = items.Select(a => new AdminJobApplicationListItemDto
        {
            Id = a.Id,
            FullName = a.FullName,
            Email = a.Email,
            Phone = a.Phone,
            Position = a.Position,
            CreatedAtUtc = a.CreatedAtUtc,
            AttachmentCount = a.AttachmentCount,
        }).ToList();
        return PagedResult<AdminJobApplicationListItemDto>.Create(dtos, pr, total);
    }

    public async Task<AdminJobApplicationDetailDto?> GetJobApplicationDetailAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var a = await applications.GetApplicationWithAttachmentsByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (a is null)
        {
            return null;
        }

        return new AdminJobApplicationDetailDto
        {
            Id = a.Id,
            FullName = a.FullName,
            Email = a.Email,
            Phone = a.Phone,
            Position = a.Position,
            CoverLetter = a.CoverLetter,
            CreatedAtUtc = a.CreatedAtUtc,
            Attachments = a.Attachments.Select(MapApplicationAttachment).ToList(),
        };
    }

    public async Task<AdminAttachmentFileDto?> GetJobApplicationAttachmentFileAsync(
        Guid applicationId,
        Guid attachmentId,
        CancellationToken cancellationToken = default)
    {
        var att = await applications.GetAttachmentByIdAsync(attachmentId, cancellationToken).ConfigureAwait(false);
        if (att is null || att.JobApplicationId != applicationId)
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

        return new AdminAttachmentFileDto(
            bytes,
            string.IsNullOrWhiteSpace(att.ContentType) ? "application/octet-stream" : att.ContentType,
            att.OriginalFileName);
    }

    public async Task<PagedResult<AdminHttpRequestLogListItemDto>> GetHttpRequestLogsAsync(
        int page,
        int pageSize,
        int? statusCode,
        string? httpMethod,
        string? pathContains,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default)
    {
        var pr = new PageRequest(page, pageSize);
        var (items, total) = await httpLogs
            .GetPagedAsync(pr.Skip, pr.PageSize, statusCode, httpMethod, pathContains, fromUtc, toUtc, cancellationToken)
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

    public async Task<AdminContentLocaleDetailDto> GetContentLocaleDetailAsync(string locale, CancellationToken cancellationToken = default)
    {
        var normalized = (locale ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new InvalidOperationException(loc.Get("Validation.Admin.Content.LocaleRequired"));
        }
        var items = await siteContent.GetLocalizedStringsByLocaleAsync(normalized, cancellationToken).ConfigureAwait(false);
        return new AdminContentLocaleDetailDto
        {
            Locale = normalized,
            Items = items
                .OrderBy(x => x.StringKey, StringComparer.Ordinal)
                .Select(x => new AdminContentStringRowDto { Key = x.StringKey, Value = x.Content })
                .ToList(),
        };
    }

    public async Task<OperationResult<AdminContentLocaleDetailDto>> UpsertContentLocaleAsync(
        AdminContentBulkUpsertRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var locale = request.Locale?.Trim().ToLowerInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(locale))
        {
            return OperationResult<AdminContentLocaleDetailDto>.Validation(CreateValidation("locale", "Validation.Admin.Content.LocaleRequired"));
        }

        var normalizedItems = (request.Items ?? Array.Empty<AdminContentStringRowDto>())
            .Where(x => !string.IsNullOrWhiteSpace(x.Key))
            .Select(x => new AdminContentStringRowDto
            {
                Key = x.Key.Trim(),
                Value = x.Value ?? string.Empty,
            })
            .ToList();
        var duplicateKeys = normalizedItems
            .GroupBy(x => x.Key, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicateKeys.Count > 0)
        {
            return OperationResult<AdminContentLocaleDetailDto>.Validation(
                CreateValidation("items", $"{loc.Get("Validation.Admin.Content.DuplicateKeysPrefix")} {string.Join(", ", duplicateKeys.Take(5))}"));
        }

        var rows = normalizedItems
            .Select(x => new SiteLocalizedStringEntity
            {
                Locale = locale,
                StringKey = x.Key,
                Content = x.Value,
            })
            .ToList();
        await siteContent.UpsertLocalizedStringsAsync(locale, rows, cancellationToken).ConfigureAwait(false);
        var dto = await GetContentLocaleDetailAsync(locale, cancellationToken).ConfigureAwait(false);
        return OperationResult<AdminContentLocaleDetailDto>.Ok(dto);
    }

    public async Task<IReadOnlyList<AdminContentAuditRowDto>> GetRecentContentAuditAsync(
        string locale,
        int take,
        CancellationToken cancellationToken = default)
    {
        var normalized = (locale ?? string.Empty).Trim().ToLowerInvariant();
        var (items, _) = await httpLogs.GetPagedAsync(
            0,
            Math.Clamp(take, 1, 100),
            null,
            null,
            "content",
            null,
            null,
            cancellationToken).ConfigureAwait(false);

        return items
            .Where(x => string.IsNullOrWhiteSpace(normalized) || x.Path.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .Select(x => new AdminContentAuditRowDto
            {
                Id = x.Id,
                OccurredAtUtc = x.OccurredAtUtc,
                Path = x.Path,
                StatusCode = x.StatusCode,
                UserEmail = x.UserEmail,
                ActionTitle = x.ActionTitle,
            })
            .ToList();
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

    private static AdminJobApplicationAttachmentDto MapApplicationAttachment(JobApplicationAttachment a) => new()
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

    private async Task<AdminUserDetailDto> BuildUserDetailDtoAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await users.GetUserByIdWithRolesAsync(id, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("User not found.");
        var row = MapUserListItem(user);
        return new AdminUserDetailDto
        {
            Id = row.Id,
            Email = row.Email,
            DisplayName = row.DisplayName,
            IsActive = row.IsActive,
            CreatedAtUtc = row.CreatedAtUtc,
            Roles = row.Roles,
            NormalizedEmail = user.NormalizedEmail,
        };
    }

    private static IReadOnlyDictionary<string, string[]> CreateValidation(string key, string message) =>
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            [key] = [message],
        };
}
