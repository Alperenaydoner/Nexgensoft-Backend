using CoreService.Contact.DTOs;

namespace CoreService.Contact.Services;

public interface IContactService
{
    /// <summary>
    /// Başarıda <paramref name="validationErrors"/> null ve <paramref name="messageId"/> dolu.
    /// Ek doğrulama hatasında <paramref name="messageId"/> null ve <paramref name="validationErrors"/> dolu.
    /// </summary>
    Task<(Guid? MessageId, IDictionary<string, string[]>? ValidationErrors)> SubmitAsync(
        ContactSubmitRequest request,
        CancellationToken cancellationToken = default);
}
