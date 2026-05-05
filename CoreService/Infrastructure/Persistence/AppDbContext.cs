using CoreService.Audit.Domain.Entities;
using CoreService.Auth.Domain.Entities;
using CoreService.Contact.Domain.Entities;
using CoreService.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    public DbSet<ContactAttachment> ContactAttachments => Set<ContactAttachment>();

    public DbSet<SiteContentBundleEntity> SiteContentBundles => Set<SiteContentBundleEntity>();

    public DbSet<SiteLocalizedStringEntity> SiteLocalizedStrings => Set<SiteLocalizedStringEntity>();

    public DbSet<HttpRequestLog> HttpRequestLogs => Set<HttpRequestLog>();

    public DbSet<AppUser> AppUsers => Set<AppUser>();

    public DbSet<AppRole> AppRoles => Set<AppRole>();

    public DbSet<AppUserRole> AppUserRoles => Set<AppUserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
