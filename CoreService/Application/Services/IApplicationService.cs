using CoreService.Application.DTOs.Requests;

namespace CoreService.Application.Services;

public interface IApplicationService
{
    Task<IReadOnlyList<string>> GetPositionOptionsAsync(CancellationToken cancellationToken = default);

    Task<(Guid? ApplicationId, IDictionary<string, string[]>? ValidationErrors)> SubmitAsync(
        ApplicationSubmitRequest request,
        CancellationToken cancellationToken = default);
}
