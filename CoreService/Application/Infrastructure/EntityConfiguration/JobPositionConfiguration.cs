using CoreService.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreService.Application.Infrastructure.EntityConfiguration;

public class JobPositionConfiguration : IEntityTypeConfiguration<JobPosition>
{
    public void Configure(EntityTypeBuilder<JobPosition> builder)
    {
        builder.ToTable("job_positions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.SortOrder).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasData(
            new JobPosition { Id = Guid.Parse("5FCBA5FD-2C26-4634-A8A4-C9892A2A2A11"), Name = "Asistan", IsActive = true, SortOrder = 10 },
            new JobPosition { Id = Guid.Parse("537DA80B-3FDE-4EC0-9946-84D2EFD2D214"), Name = "Sofor", IsActive = true, SortOrder = 20 },
            new JobPosition { Id = Guid.Parse("3C26D21F-BA4D-4E8A-9C89-296A6C0A0381"), Name = "Yazilim Gelistirici", IsActive = true, SortOrder = 30 },
            new JobPosition { Id = Guid.Parse("B089A454-5D9B-4267-84C4-BB3F02579D88"), Name = "Frontend Gelistirici", IsActive = true, SortOrder = 40 },
            new JobPosition { Id = Guid.Parse("8DC3A70A-5719-4EE4-9A95-9D471C61139E"), Name = "DevOps Muhendisi", IsActive = true, SortOrder = 50 },
            new JobPosition { Id = Guid.Parse("C2AF8E5E-C91D-4EA2-A34E-75257D7FB016"), Name = "Stajyer", IsActive = true, SortOrder = 60 }
        );
    }
}
