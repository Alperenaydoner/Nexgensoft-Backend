using CoreService.Application.Domain.Entities;
using CoreService.Application.DTOs;
using CoreService.Application.DTOs.Requests;
using CoreService.Application.Infrastructure.Repositories;
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

    public async Task<(Guid? ApplicationId, IDictionary<string, string[]>? ValidationErrors)> SubmitAsync(
        ApplicationSubmitRequest request,
        CancellationToken cancellationToken = default)
    {
        var options = uploadOptions.Value;
        var (attachments, errors) = TryBuildAttachments(request.Attachments, options);
        if (errors is not null)
        {
            return (null, errors);
        }

        var application = new JobApplication
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Position = request.Position.Trim(),
            CoverLetter = string.IsNullOrWhiteSpace(request.CoverLetter) ? null : request.CoverLetter.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
        };

        foreach (var a in attachments)
        {
            a.JobApplicationId = application.Id;
        }

        await repository.AddApplicationWithAttachmentsAsync(application, attachments, cancellationToken);
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
            errors["attachments"] = [$"At most {options.MaxFilesPerApplication} file(s) allowed."];
            return ([], errors);
        }

        var allowed = new HashSet<string>(options.AllowedExtensions, StringComparer.OrdinalIgnoreCase);
        var result = new List<JobApplicationAttachment>();
        foreach (var item in list)
        {
            var ext = Path.GetExtension(item.FileName);
            if (string.IsNullOrEmpty(ext) || !allowed.Contains(ext))
            {
                errors["attachments"] = [$"File type not allowed: {ext}"];
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
                errors["attachments"] = ["Invalid Base64 payload."];
                return ([], errors);
            }

            if (bytes.LongLength > options.MaxBytesPerFile)
            {
                errors["attachments"] = [$"File exceeds maximum decoded size ({options.MaxBytesPerFile} bytes)."];
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
