using CoreService.Application.Domain.Entities;
using CoreService.Application.DTOs;
using CoreService.Application.DTOs.Requests;
using CoreService.Application.DTOs.Responses;
using CoreService.Application.Infrastructure.Repositories;
using CoreService.Common.Validation;
using Microsoft.Extensions.Options;

namespace CoreService.Application.Services;

public class ApplicationService(
    IApplicationRepository repository,
    IOptions<ApplicationUploadOptions> uploadOptions) : IApplicationService
{
    public async Task<IReadOnlyList<string>> GetPositionOptionsAsync(CancellationToken cancellationToken = default)
    {
        var items = await repository.GetActivePositionNamesAsync(cancellationToken);
        return items.Count > 0 ? items : ["Asistan", "Sofor", "Yazilim Gelistirici"];
    }

    public async Task<ApplicationByCodeResponse?> GetByCodeAsync(Guid applicationCode, CancellationToken cancellationToken = default)
    {
        var application = await repository.GetApplicationWithAttachmentsByIdAsync(applicationCode, cancellationToken);
        if (application is null)
        {
            return null;
        }

        return new ApplicationByCodeResponse
        {
            ApplicationCode = application.Id,
            FullName = application.FullName,
            Email = application.Email,
            Phone = application.Phone,
            Position = application.Position,
            CoverLetter = application.CoverLetter,
        };
    }

    public async Task<(Guid? ApplicationId, IDictionary<string, string[]>? ValidationErrors)> SubmitAsync(
        ApplicationSubmitRequest request,
        CancellationToken cancellationToken = default)
    {
        var qualityErrors = ApplicantContentValidationHelper.ValidateFullNameAndEmail(request.FullName, request.Email);
        if (qualityErrors is not null)
        {
            return (null, qualityErrors);
        }

        var options = uploadOptions.Value;
        var (attachments, errors) = TryBuildAttachments(request.Attachments, options);
        if (errors is not null)
        {
            return (null, errors);
        }

        if (attachments.Count < options.MinFilesPerApplication)
        {
            return (null, new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["attachments"] = ["Validation.Application.AttachmentsMinCount"],
            });
        }

        var application = new JobApplication
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Position = request.Position.Trim(),
            CoverLetter = request.CoverLetter.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
        };

        foreach (var a in attachments)
        {
            a.JobApplicationId = application.Id;
        }

        await repository.AddApplicationWithAttachmentsAsync(application, attachments, cancellationToken);
        return (application.Id, null);
    }

    public async Task<(Guid? ApplicationId, IDictionary<string, string[]>? ValidationErrors)> UpdateByCodeAsync(
        Guid applicationCode,
        ApplicationUpdateByCodeRequest request,
        CancellationToken cancellationToken = default)
    {
        var application = await repository.GetApplicationByIdForUpdateAsync(applicationCode, cancellationToken);
        if (application is null)
        {
            return (null, new Dictionary<string, string[]>
            {
                ["applicationCode"] = ["Validation.Application.NotFound"],
            });
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            application.FullName = request.FullName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            application.Email = request.Email.Trim();
        }

        if (request.Phone is not null)
        {
            application.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Position))
        {
            application.Position = request.Position.Trim();
        }

        if (request.CoverLetter is not null)
        {
            application.CoverLetter = string.IsNullOrWhiteSpace(request.CoverLetter) ? null : request.CoverLetter.Trim();
        }

        var options = uploadOptions.Value;
        var (attachments, buildErrors) = TryBuildAttachments(request.Attachments, options);
        if (buildErrors is not null)
        {
            return (null, buildErrors);
        }

        if (attachments.Count > 0 && attachments.Count < options.MinFilesPerApplication)
        {
            return (null, new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["attachments"] = ["Validation.Application.AttachmentsMinCount"],
            });
        }

        if (string.IsNullOrWhiteSpace(application.CoverLetter))
        {
            return (null, new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["coverLetter"] = ["Validation.Application.CoverLetterRequired"],
            });
        }

        if (application.CoverLetter.Length < 10)
        {
            return (null, new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["coverLetter"] = ["Validation.Application.CoverLetterMinLength"],
            });
        }

        if (string.IsNullOrWhiteSpace(application.Position))
        {
            return (null, new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["position"] = ["Validation.Application.PositionRequired"],
            });
        }

        var updateQualityErrors = ApplicantContentValidationHelper.ValidateFullNameAndEmail(application.FullName, application.Email);
        if (updateQualityErrors is not null)
        {
            return (null, updateQualityErrors);
        }

        foreach (var attachment in attachments)
        {
            attachment.JobApplicationId = application.Id;
            application.Attachments.Add(attachment);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return (application.Id, null);
    }

    private static (List<JobApplicationAttachment> Attachments, Dictionary<string, string[]>? Errors) TryBuildAttachments(
        List<ApplicationAttachmentSubmitDto>? items,
        ApplicationUploadOptions options)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        var list = items?.Where(static a => !string.IsNullOrWhiteSpace(a.Base64)).ToList() ?? [];
        if (list.Count > options.MaxFilesPerApplication)
        {
            errors["attachments"] = ["Validation.Application.AttachmentsTooMany"];
            return ([], errors);
        }

        var allowed = new HashSet<string>(options.AllowedExtensions, StringComparer.OrdinalIgnoreCase);
        var result = new List<JobApplicationAttachment>();
        foreach (var item in list)
        {
            var ext = Path.GetExtension(item.FileName);
            if (string.IsNullOrEmpty(ext) || !allowed.Contains(ext))
            {
                errors["attachments"] = ["Validation.Application.AttachmentExtensionNotAllowed"];
                return ([], errors);
            }

            var raw = NormalizeBase64Payload(item.Base64);
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(raw);
            }
            catch (FormatException)
            {
                errors["attachments"] = ["Validation.Application.AttachmentInvalidPayload"];
                return ([], errors);
            }

            if (bytes.LongLength > options.MaxBytesPerFile)
            {
                errors["attachments"] = ["Validation.Application.AttachmentTooLarge"];
                return ([], errors);
            }

            result.Add(
                new JobApplicationAttachment
                {
                    Id = Guid.NewGuid(),
                    JobApplicationId = default,
                    OriginalFileName = Path.GetFileName(item.FileName).Trim(),
                    ContentType = string.IsNullOrWhiteSpace(item.ContentType)
                        ? "application/octet-stream"
                        : item.ContentType.Trim(),
                    SizeBytes = bytes.LongLength,
                    ContentBase64 = Convert.ToBase64String(bytes),
                });
        }

        return (result, null);
    }

    private static string NormalizeBase64Payload(string input)
    {
        var s = input.Trim();
        var comma = s.IndexOf("base64,", StringComparison.OrdinalIgnoreCase);
        if (comma >= 0)
        {
            s = s[(comma + "base64,".Length)..];
        }

        return s.Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);
    }
}
