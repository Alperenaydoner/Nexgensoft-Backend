using CoreService.Application.Domain.Entities;

namespace CoreService.Application.Infrastructure.Repositories;

public interface IApplicationRepository
{
    Task AddApplicationWithAttachmentsAsync(
        JobApplication application,
        IReadOnlyList<JobApplicationAttachment> attachments,
        CancellationToken cancellationToken = default);

    Task<int> CountApplicationsAsync(CancellationToken cancellationToken = default);

    Task<int> CountAttachmentsAsync(CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<JobApplication> Items, int TotalCount)> GetApplicationsPagedAsync(
        int skip,
        int take,
        string? query,
        string? position,
        bool? hasAttachments,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default);

    Task<JobApplication?> GetApplicationWithAttachmentsByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<JobApplicationAttachment?> GetAttachmentByIdAsync(Guid attachmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetActivePositionNamesAsync(CancellationToken cancellationToken = default);
}
