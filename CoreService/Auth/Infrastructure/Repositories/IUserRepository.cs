using CoreService.Auth.Domain.Entities;

namespace CoreService.Auth.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<bool> AnyRoleWithNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default);

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
        string? query,
        bool? isActive,
        string? role,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<AppRole> Items, int TotalCount)> GetRolesPagedWithUserCountsAsync(
        int skip,
        int take,
        string? query,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default);

    Task<AppUser?> GetUserByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AppRole?> GetRoleByIdWithUserCountsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetAllRoleNamesAsync(CancellationToken cancellationToken = default);

    Task<AppUser> CreateUserAsync(AppUser user, IReadOnlyList<string> roleNames, CancellationToken cancellationToken = default);

    Task<AppUser?> UpdateUserAsync(
        Guid id,
        string email,
        string displayName,
        bool isActive,
        string? password,
        IReadOnlyList<string> roleNames,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> DeleteUsersAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default);

    Task<AppRole> CreateRoleAsync(AppRole role, CancellationToken cancellationToken = default);

    Task<AppRole?> UpdateRoleAsync(Guid id, string name, CancellationToken cancellationToken = default);

    Task<bool> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default);
}
