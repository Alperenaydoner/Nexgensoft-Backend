using CoreService.Auth.Domain.Entities;
using CoreService.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreService.Content.Infrastructure.EntityConfiguration;

public class SiteContentBundleEntityConfiguration : IEntityTypeConfiguration<SiteContentBundleEntity>
{
    public void Configure(EntityTypeBuilder<SiteContentBundleEntity> builder)
    {
        builder.ToTable("site_content_bundles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Locale).HasMaxLength(10).IsRequired();
        builder.Property(x => x.PayloadJson).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.Locale).IsUnique();
        builder.HasIndex(x => new { x.IsActive, x.IsDeleted });
        builder.HasIndex(x => x.UserId);
    }
}
