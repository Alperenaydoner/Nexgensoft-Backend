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

        builder.HasIndex(x => x.Locale).IsUnique();
    }
}
