using CoreService.Contact.Domain.Entities;
using CoreService.Contact.DTOs;
using CoreService.Contact.DTOs.Requests;
using CoreService.Contact.Infrastructure.Repositories;
using CoreService.Common;
using CoreService.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

namespace CoreService.Contact.Services;

public class ContactService(
    IContactRepository repository,
    IUnitOfWork unitOfWork,
    IOptions<ContactUploadOptions> uploadOptions) : IContactService
{
    public async Task<OperationResult<Guid>> SubmitAsync(
        ContactSubmitRequest request,
        CancellationToken cancellationToken = default)
    {
        var options = uploadOptions.Value;
        var (attachments, errors) = TryBuildAttachments(request.Attachments, options);
        if (errors is not null)
        {
            return OperationResult<Guid>.Validation(errors);
        }

        var message = new ContactMessage
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            Company = string.IsNullOrWhiteSpace(request.Company) ? null : request.Company.Trim(),
            Message = request.Message.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
        };

        foreach (var a in attachments)
        {
            a.ContactMessageId = message.Id;
        }

        await repository.AddMessageWithAttachmentsAsync(message, attachments, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return OperationResult<Guid>.Ok(message.Id);
    }

    private static (List<ContactAttachment> Attachments, Dictionary<string, string[]>? Errors) TryBuildAttachments(
        List<ContactAttachmentSubmitDto>? items,
        ContactUploadOptions options)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        var list = items?.Where(static a => !string.IsNullOrWhiteSpace(a.Base64)).ToList() ?? [];
        if (list.Count > options.MaxFilesPerMessage)
        {
            errors["attachments"] =
            [
                $"At most {options.MaxFilesPerMessage} file(s) allowed.",
            ];
            return ([], errors);
        }

        var allowed = new HashSet<string>(options.AllowedExtensions, StringComparer.OrdinalIgnoreCase);
        var result = new List<ContactAttachment>();
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
                errors["attachments"] =
                [
                    $"File exceeds maximum decoded size ({options.MaxBytesPerFile} bytes).",
                ];
                return ([], errors);
            }

            var id = Guid.NewGuid();
            result.Add(
                new ContactAttachment
                {
                    Id = id,
                    ContactMessageId = default,
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

    /// <summary>data:…;base64, önekini atar; boşlukları kırpar.</summary>
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
