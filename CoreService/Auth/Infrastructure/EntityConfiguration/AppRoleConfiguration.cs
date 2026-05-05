using CoreService.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreService.Auth.Infrastructure.EntityConfiguration;

public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.ToTable("app_roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(64).IsRequired();
        builder.Property(x => x.NormalizedName).HasMaxLength(64).IsRequired();

        builder.HasIndex(x => x.NormalizedName).IsUnique();
    }
}
