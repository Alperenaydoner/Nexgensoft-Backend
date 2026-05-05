using CoreService.Auth.DTOs.Requests;
using CoreService.Auth.DTOs.Responses;

namespace CoreService.Auth.Services;

public interface IAuthService
{
    /// <summary>Başarısız girişte <c>null</c> (aynı hata mesajı ile 401).</summary>
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
