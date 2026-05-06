using System.Reflection;
using CoreService.Application.Domain.Entities;
using CoreService.Audit.Domain.Entities;
using CoreService.Auth.Domain.Entities;
using CoreService.Common;
using CoreService.Contact.Domain.Entities;
using CoreService.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    public DbSet<ContactAttachment> ContactAttachments => Set<ContactAttachment>();

    public DbSet<JobApplication> JobApplications => Set<JobApplication>();

    public DbSet<JobApplicationAttachment> JobApplicationAttachments => Set<JobApplicationAttachment>();

    public DbSet<JobPosition> JobPositions => Set<JobPosition>();

    public DbSet<SiteContentBundleEntity> SiteContentBundles => Set<SiteContentBundleEntity>();

    public DbSet<SiteLocalizedStringEntity> SiteLocalizedStrings => Set<SiteLocalizedStringEntity>();

    public DbSet<HttpRequestLog> HttpRequestLogs => Set<HttpRequestLog>();

    public DbSet<AppUser> AppUsers => Set<AppUser>();

    public DbSet<AppRole> AppRoles => Set<AppRole>();

    public DbSet<AppUserRole> AppUserRoles => Set<AppUserRole>();

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampAuditableFields();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampAuditableFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        ApplySoftDeleteQueryFilter(modelBuilder);
    }

    private void StampAuditableFields()
    {
        var utcNow = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditableTimestamps auditable)
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    if (auditable.CreatedAtUtc == default)
                    {
                        auditable.CreatedAtUtc = utcNow;
                    }
                    break;
                case EntityState.Modified:
                    auditable.UpdatedAtUtc = utcNow;
                    break;
            }
        }
    }

    private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;
            if (clr is null || clr.IsAbstract || !typeof(ISoftDeletable).IsAssignableFrom(clr))
            {
                continue;
            }

            var method = typeof(AppDbContext).GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.Static | BindingFlags.NonPublic)!
                .MakeGenericMethod(clr);
            method.Invoke(null, [modelBuilder]);
        }
    }

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ISoftDeletable
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.IsActive && !e.IsDeleted);
    }
}
