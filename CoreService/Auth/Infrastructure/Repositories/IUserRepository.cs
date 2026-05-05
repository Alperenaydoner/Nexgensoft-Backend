using CoreService.Auth.Domain.Entities;

namespace CoreService.Auth.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<AppUser?> GetByNormalizedEmailWithRolesAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    Task<bool> AnyUserWithNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    Task AddUserAsync(AppUser user, CancellationToken cancellationToken = default);

    Task<AppRole?> GetRoleByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default);

    Task AddRoleAsync(AppRole role, CancellationToken cancellationToken = default);

    Task AddUserRoleAsync(AppUserRole link, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<int> CountUsersAsync(CancellationToken cancellationToken = default);

    Task<int> CountRolesAsync(CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<AppUser> Items, int TotalCount)> GetUsersPagedWithRolesAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<AppRole> Items, int TotalCount)> GetRolesPagedWithUserCountsAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<AppUser?> GetUserByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default);
}
