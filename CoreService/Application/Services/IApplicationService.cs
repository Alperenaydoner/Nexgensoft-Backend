using CoreService.Application.DTOs.Requests;
using CoreService.Application.DTOs.Responses;
using CoreService.Common;

namespace CoreService.Application.Services;

public interface IApplicationService
{
    Task<IReadOnlyList<string>> GetPositionOptionsAsync(CancellationToken cancellationToken = default);

    Task<ApplicationByCodeResponse?> GetByCodeAsync(Guid applicationCode, CancellationToken cancellationToken = default);

    Task<OperationResult<Guid>> SubmitAsync(
        ApplicationSubmitRequest request,
        CancellationToken cancellationToken = default);

    Task<OperationResult<Guid>> UpdateByCodeAsync(
        Guid applicationCode,
        ApplicationUpdateByCodeRequest request,
        CancellationToken cancellationToken = default);
}
