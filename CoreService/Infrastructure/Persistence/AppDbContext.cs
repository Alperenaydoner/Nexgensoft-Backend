using CoreService.Contact.Domain;
using CoreService.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    public DbSet<ContactAttachment> ContactAttachments => Set<ContactAttachment>();

    public DbSet<SiteContentBundleEntity> SiteContentBundles => Set<SiteContentBundleEntity>();

    public DbSet<SiteLocalizedStringEntity> SiteLocalizedStrings => Set<SiteLocalizedStringEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
