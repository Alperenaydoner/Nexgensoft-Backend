using CoreService.Contact.DTOs.Requests;
using CoreService.Common;

namespace CoreService.Contact.Services;

public interface IContactService
{
    Task<OperationResult<Guid>> SubmitAsync(
        ContactSubmitRequest request,
        CancellationToken cancellationToken = default);
}
