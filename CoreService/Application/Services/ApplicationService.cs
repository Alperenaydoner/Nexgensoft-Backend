using CoreService.Application.Domain.Entities;
using CoreService.Application.DTOs;
using CoreService.Application.DTOs.Requests;
using CoreService.Application.DTOs.Responses;
using CoreService.Application.Infrastructure.Repositories;
using CoreService.Common;
using CoreService.Common.Validation;
using CoreService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CoreService.Application.Services;

public class ApplicationService(
    IApplicationRepository repository,
    IUnitOfWork unitOfWork,
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

    public async Task<OperationResult<Guid>> SubmitAsync(
        ApplicationSubmitRequest request,
        CancellationToken cancellationToken = default)
    {
        var qualityErrors = ApplicantContentValidationHelper.ValidateFullNameAndEmail(request.FullName, request.Email);
        if (qualityErrors is not null)
        {
            return OperationResult<Guid>.Validation(qualityErrors);
        }

        var options = uploadOptions.Value;
        var (attachments, errors) = TryBuildAttachments(request.Attachments, options);
        if (errors is not null)
        {
            return OperationResult<Guid>.Validation(errors);
        }

        if (attachments.Count < options.MinFilesPerApplication)
        {
            return OperationResult<Guid>.Validation(new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
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
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return OperationResult<Guid>.Ok(application.Id);
    }

    public async Task<OperationResult<Guid>> UpdateByCodeAsync(
        Guid applicationCode,
        ApplicationUpdateByCodeRequest request,
        CancellationToken cancellationToken = default)
    {
        var application = await repository.GetApplicationByIdForUpdateAsync(applicationCode, cancellationToken);
        if (application is null)
        {
            return OperationResult<Guid>.Validation(new Dictionary<string, string[]>
            {
                ["applicationCode"] = ["Validation.Application.NotFound"],
            });
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            var nextFullName = request.FullName.Trim();
            if (!string.Equals(application.FullName, nextFullName, StringComparison.Ordinal))
            {
                application.FullName = nextFullName;
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var nextEmail = request.Email.Trim();
            if (!string.Equals(application.Email, nextEmail, StringComparison.Ordinal))
            {
                application.Email = nextEmail;
            }
        }

        if (request.Phone is not null)
        {
            var nextPhone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
            if (!string.Equals(application.Phone, nextPhone, StringComparison.Ordinal))
            {
                application.Phone = nextPhone;
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Position))
        {
            var nextPosition = request.Position.Trim();
            if (!string.Equals(application.Position, nextPosition, StringComparison.Ordinal))
            {
                application.Position = nextPosition;
            }
        }

        if (request.CoverLetter is not null)
        {
            var nextCoverLetter = string.IsNullOrWhiteSpace(request.CoverLetter) ? null : request.CoverLetter.Trim();
            if (!string.Equals(application.CoverLetter, nextCoverLetter, StringComparison.Ordinal))
            {
                application.CoverLetter = nextCoverLetter;
            }
        }

        var options = uploadOptions.Value;
        var (attachments, buildErrors) = TryBuildAttachments(request.Attachments, options);
        if (buildErrors is not null)
        {
            return OperationResult<Guid>.Validation(buildErrors);
        }

        if (string.IsNullOrWhiteSpace(application.CoverLetter))
        {
            return OperationResult<Guid>.Validation(new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["coverLetter"] = ["Validation.Application.CoverLetterRequired"],
            });
        }

        if (application.CoverLetter.Length < 10)
        {
            return OperationResult<Guid>.Validation(new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["coverLetter"] = ["Validation.Application.CoverLetterMinLength"],
            });
        }

        if (string.IsNullOrWhiteSpace(application.Position))
        {
            return OperationResult<Guid>.Validation(new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["position"] = ["Validation.Application.PositionRequired"],
            });
        }

        var updateQualityErrors = ApplicantContentValidationHelper.ValidateFullNameAndEmail(application.FullName, application.Email);
        if (updateQualityErrors is not null)
        {
            return OperationResult<Guid>.Validation(updateQualityErrors);
        }

        foreach (var attachment in attachments)
        {
            attachment.JobApplicationId = application.Id;
        }

        repository.AddAttachments(attachments);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return OperationResult<Guid>.Validation(new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["applicationCode"] = ["Validation.Application.NotFound"],
            });
        }
        return OperationResult<Guid>.Ok(application.Id);
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
