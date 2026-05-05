using CoreService.Auth.Domain.Entities;
using CoreService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Auth.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<bool> AnyRoleWithNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default) =>
        db.AppRoles.AsNoTracking().AnyAsync(r => r.NormalizedName == normalizedName, cancellationToken);

    public Task<AppUser?> GetByNormalizedEmailWithRolesAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
        db.AppUsers.AsNoTracking()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

    public Task<bool> AnyUserWithNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
        db.AppUsers.AsNoTracking().AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

    public async Task AddUserAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        db.AppUsers.Add(user);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<AppRole?> GetRoleByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default) =>
        db.AppRoles.AsNoTracking().FirstOrDefaultAsync(r => r.NormalizedName == normalizedName, cancellationToken);

    public async Task AddRoleAsync(AppRole role, CancellationToken cancellationToken = default)
    {
        db.AppRoles.Add(role);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddUserRoleAsync(AppUserRole link, CancellationToken cancellationToken = default)
    {
        db.AppUserRoles.Add(link);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);

    public Task<int> CountUsersAsync(CancellationToken cancellationToken = default) =>
        db.AppUsers.AsNoTracking().CountAsync(cancellationToken);

    public Task<int> CountRolesAsync(CancellationToken cancellationToken = default) =>
        db.AppRoles.AsNoTracking().CountAsync(cancellationToken);

    public async Task<(IReadOnlyList<AppUser> Items, int TotalCount)> GetUsersPagedWithRolesAsync(
        int skip,
        int take,
        string? query,
        bool? isActive,
        string? role,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default)
    {
        var q = db.AppUsers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalizedQuery = query.Trim().ToUpperInvariant();
            q = q.Where(u =>
                u.NormalizedEmail.Contains(normalizedQuery) ||
                u.DisplayName.ToUpper().Contains(normalizedQuery));
        }

        if (isActive.HasValue)
        {
            q = q.Where(u => u.IsActive == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            var normalizedRole = role.Trim().ToUpperInvariant();
            q = q.Where(u => u.UserRoles.Any(ur => ur.Role.NormalizedName == normalizedRole));
        }

        var asc = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
        q = (sortBy ?? string.Empty).ToLowerInvariant() switch
        {
            "email" => asc ? q.OrderBy(u => u.Email) : q.OrderByDescending(u => u.Email),
            "displayname" => asc ? q.OrderBy(u => u.DisplayName) : q.OrderByDescending(u => u.DisplayName),
            "active" => asc ? q.OrderBy(u => u.IsActive) : q.OrderByDescending(u => u.IsActive),
            _ => asc ? q.OrderBy(u => u.CreatedAtUtc) : q.OrderByDescending(u => u.CreatedAtUtc),
        };

        var total = await q.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await q
            .Skip(skip)
            .Take(take)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return (items, total);
    }

    public async Task<(IReadOnlyList<AppRole> Items, int TotalCount)> GetRolesPagedWithUserCountsAsync(
        int skip,
        int take,
        string? query,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default)
    {
        var q = db.AppRoles.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalizedQuery = query.Trim().ToUpperInvariant();
            q = q.Where(r =>
                r.NormalizedName.Contains(normalizedQuery) ||
                r.Name.ToUpper().Contains(normalizedQuery));
        }

        var asc = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
        q = (sortBy ?? string.Empty).ToLowerInvariant() switch
        {
            "users" => asc ? q.OrderBy(r => r.UserRoles.Count) : q.OrderByDescending(r => r.UserRoles.Count),
            "normalizedname" => asc ? q.OrderBy(r => r.NormalizedName) : q.OrderByDescending(r => r.NormalizedName),
            _ => asc ? q.OrderBy(r => r.Name) : q.OrderByDescending(r => r.Name),
        };

        var total = await q.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await q
            .Skip(skip)
            .Take(take)
            .Include(r => r.UserRoles)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return (items, total);
    }

    public Task<AppUser?> GetUserByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.AppUsers.AsNoTracking()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<AppRole?> GetRoleByIdWithUserCountsAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.AppRoles.AsNoTracking()
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<IReadOnlyList<string>> GetAllRoleNamesAsync(CancellationToken cancellationToken = default) =>
        db.AppRoles.AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => r.Name)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<string>)t.Result, cancellationToken);

    public async Task<AppUser> CreateUserAsync(AppUser user, IReadOnlyList<string> roleNames, CancellationToken cancellationToken = default)
    {
        db.AppUsers.Add(user);
        if (roleNames.Count > 0)
        {
            var normalizedRoles = roleNames.Select(x => x.Trim().ToUpperInvariant()).Distinct().ToList();
            var roles = await db.AppRoles.Where(r => normalizedRoles.Contains(r.NormalizedName)).ToListAsync(cancellationToken);
            var links = roles.Select(r => new AppUserRole { UserId = user.Id, RoleId = r.Id }).ToList();
            if (links.Count > 0)
            {
                db.AppUserRoles.AddRange(links);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        return (await GetUserByIdWithRolesAsync(user.Id, cancellationToken).ConfigureAwait(false))!;
    }

    public async Task<AppUser?> UpdateUserAsync(
        Guid id,
        string email,
        string displayName,
        bool isActive,
        string? password,
        IReadOnlyList<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        var user = await db.AppUsers.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
        {
            return null;
        }

        user.Email = email;
        user.NormalizedEmail = email.Trim().ToUpperInvariant();
        user.DisplayName = displayName;
        user.IsActive = isActive;
        if (!string.IsNullOrWhiteSpace(password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        var normalizedRoles = roleNames.Select(x => x.Trim().ToUpperInvariant()).Distinct().ToList();
        var roles = await db.AppRoles.Where(r => normalizedRoles.Contains(r.NormalizedName)).ToListAsync(cancellationToken);
        db.AppUserRoles.RemoveRange(user.UserRoles);
        if (roles.Count > 0)
        {
            db.AppUserRoles.AddRange(roles.Select(r => new AppUserRole { UserId = user.Id, RoleId = r.Id }));
        }

        await db.SaveChangesAsync(cancellationToken);
        return await GetUserByIdWithRolesAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await db.AppUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
        {
            return false;
        }

        db.AppUsers.Remove(user);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> DeleteUsersAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return 0;
        }

        var users = await db.AppUsers.Where(u => ids.Contains(u.Id)).ToListAsync(cancellationToken);
        if (users.Count == 0)
        {
            return 0;
        }

        db.AppUsers.RemoveRange(users);
        await db.SaveChangesAsync(cancellationToken);
        return users.Count;
    }

    public async Task<AppRole> CreateRoleAsync(AppRole role, CancellationToken cancellationToken = default)
    {
        db.AppRoles.Add(role);
        await db.SaveChangesAsync(cancellationToken);
        return role;
    }

    public async Task<AppRole?> UpdateRoleAsync(Guid id, string name, CancellationToken cancellationToken = default)
    {
        var role = await db.AppRoles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (role is null)
        {
            return null;
        }

        role.Name = name;
        role.NormalizedName = name.Trim().ToUpperInvariant();
        await db.SaveChangesAsync(cancellationToken);
        return await GetRoleByIdWithUserCountsAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await db.AppRoles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (role is null)
        {
            return false;
        }

        db.AppRoles.Remove(role);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
