using CoreService.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreService.Content.Infrastructure.EntityConfiguration;

public class SiteLocalizedStringEntityConfiguration : IEntityTypeConfiguration<SiteLocalizedStringEntity>
{
    public void Configure(EntityTypeBuilder<SiteLocalizedStringEntity> builder)
    {
        builder.ToTable("site_localized_strings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Locale).HasMaxLength(10).IsRequired();
        builder.Property(x => x.StringKey).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Content).IsRequired();

        builder.HasIndex(x => new { x.Locale, x.StringKey }).IsUnique();
    }
}
