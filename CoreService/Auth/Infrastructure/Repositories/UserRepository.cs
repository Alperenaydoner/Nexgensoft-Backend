using CoreService.Auth.Domain.Entities;
using CoreService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Auth.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
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
        CancellationToken cancellationToken = default)
    {
        var q = db.AppUsers.AsNoTracking();
        var total = await q.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await q
            .OrderByDescending(u => u.CreatedAtUtc)
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
        CancellationToken cancellationToken = default)
    {
        var q = db.AppRoles.AsNoTracking();
        var total = await q.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await q
            .OrderBy(r => r.Name)
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
}
