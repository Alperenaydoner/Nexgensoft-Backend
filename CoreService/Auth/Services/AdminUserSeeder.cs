using CoreService.Auth.Domain.Entities;
using CoreService.Auth.DTOs;
using CoreService.Auth.Infrastructure.Repositories;
using Microsoft.Extensions.Options;

namespace CoreService.Auth.Services;

public class AdminUserSeeder(
    IUserRepository users,
    IOptions<AdminUserSeedOptions> seedOptions,
    IConfiguration configuration,
    ILogger<AdminUserSeeder> logger) : IAdminUserSeeder
{
    private const string AdminRoleName = "Admin";

    public async Task SeedAdminIfEnabledAsync(CancellationToken cancellationToken = default)
    {
        if (!configuration.GetValue("Seed:EnableAdminUserSeed", false))
        {
            return;
        }

        var opt = seedOptions.Value;
        if (string.IsNullOrWhiteSpace(opt.Password))
        {
            logger.LogWarning("Seed:EnableAdminUserSeed açık ancak Seed:AdminUser:Password boş; admin oluşturulmadı.");
            return;
        }

        var email = opt.Email.Trim();
        var normalizedEmail = email.ToUpperInvariant();

        if (await users.AnyUserWithNormalizedEmailAsync(normalizedEmail, cancellationToken).ConfigureAwait(false))
        {
            logger.LogInformation("Admin seed atlandı: {Email} zaten var.", email);
            return;
        }

        var role = await users.GetRoleByNormalizedNameAsync(AdminRoleName.ToUpperInvariant(), cancellationToken)
            .ConfigureAwait(false);
        if (role is null)
        {
            role = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = AdminRoleName,
                NormalizedName = AdminRoleName.ToUpperInvariant(),
            };
            await users.AddRoleAsync(role, cancellationToken).ConfigureAwait(false);
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            NormalizedEmail = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(opt.Password),
            DisplayName = "Administrator",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
        };
        await users.AddUserAsync(user, cancellationToken).ConfigureAwait(false);

        await users.AddUserRoleAsync(
            new AppUserRole { UserId = user.Id, RoleId = role.Id },
            cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Admin kullanıcı seed tamamlandı: {Email}", email);
    }
}
