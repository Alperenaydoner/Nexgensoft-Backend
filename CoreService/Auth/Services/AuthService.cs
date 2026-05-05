using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoreService.Auth.Domain.Entities;
using CoreService.Auth.DTOs;
using CoreService.Auth.DTOs.Requests;
using CoreService.Auth.DTOs.Responses;
using CoreService.Auth.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CoreService.Auth.Services;

public class AuthService(IUserRepository users, IOptions<JwtOptions> jwtOptions) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeEmail(request.Email);
        var user = await users.GetByNormalizedEmailWithRolesAsync(normalized, cancellationToken).ConfigureAwait(false);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        try
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }
        }
        catch (Exception)
        {
            return null;
        }

        var roles = user.UserRoles.Select(ur => ur.Role.Name).Distinct().ToList();
        var (token, expires) = CreateAccessToken(user, roles);
        return new LoginResponse
        {
            AccessToken = token,
            ExpiresAtUtc = expires,
            User = new CurrentUserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Roles = roles,
            },
        };
    }

    private (string Token, DateTime ExpiresUtc) CreateAccessToken(AppUser user, IReadOnlyList<string> roles)
    {
        var jwt = jwtOptions.Value;
        var expires = DateTime.UtcNow.AddMinutes(jwt.AccessTokenMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString("D")),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.DisplayName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D")),
        };
        foreach (var r in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, r));
        }

        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow.AddSeconds(-30),
            expires: expires,
            signingCredentials: creds);
        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);
        return (jwtString, expires);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToUpperInvariant();
}
