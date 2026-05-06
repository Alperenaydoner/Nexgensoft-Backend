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
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.NormalizedName).IsUnique();
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => new { x.IsActive, x.IsDeleted });
        builder.HasIndex(x => x.UserId);
    }
}
