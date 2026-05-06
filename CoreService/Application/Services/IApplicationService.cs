using CoreService.Application.DTOs.Requests;
using CoreService.Application.DTOs.Responses;

namespace CoreService.Application.Services;

public interface IApplicationService
{
    Task<IReadOnlyList<string>> GetPositionOptionsAsync(CancellationToken cancellationToken = default);

    Task<ApplicationByCodeResponse?> GetByCodeAsync(Guid applicationCode, CancellationToken cancellationToken = default);

    Task<(Guid? ApplicationId, IDictionary<string, string[]>? ValidationErrors)> SubmitAsync(
        ApplicationSubmitRequest request,
        CancellationToken cancellationToken = default);

    Task<(Guid? ApplicationId, IDictionary<string, string[]>? ValidationErrors)> UpdateByCodeAsync(
        Guid applicationCode,
        ApplicationUpdateByCodeRequest request,
        CancellationToken cancellationToken = default);
}
