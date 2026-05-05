using System.Security.Claims;
using CoreService.Audit.Attributes;
using CoreService.Common;
using CoreService.Auth.DTOs.Requests;
using CoreService.Auth.DTOs.Responses;
using CoreService.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreService.Auth.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [AuditAction("AdminGiris", "Yönetici girişi", "Yönetici oturum isteği tamamlandı.")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> LoginAsync(
        [FromBody] LoginRequest body,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await authService.LoginAsync(body, cancellationToken);
        if (result is null)
        {
            return this.ProblemFromCatalog(ProblemCatalog.Auth.InvalidCredentials, "Invalid email or password.");
        }

        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(CurrentUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<CurrentUserDto> Me()
    {
        var idRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(idRaw) || !Guid.TryParse(idRaw, out var id))
        {
            return Unauthorized();
        }

        var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var displayName = User.FindFirstValue(ClaimTypes.GivenName) ?? email;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        return Ok(new CurrentUserDto
        {
            Id = id,
            Email = email,
            DisplayName = displayName,
            Roles = roles,
        });
    }
}
