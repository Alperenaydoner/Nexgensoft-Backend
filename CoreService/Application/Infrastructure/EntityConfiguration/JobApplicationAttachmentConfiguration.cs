using CoreService.Application.Domain.Entities;
using CoreService.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreService.Application.Infrastructure.EntityConfiguration;

public class JobApplicationAttachmentConfiguration : IEntityTypeConfiguration<JobApplicationAttachment>
{
    public void Configure(EntityTypeBuilder<JobApplicationAttachment> builder)
    {
        builder.ToTable("job_application_attachments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OriginalFileName).HasMaxLength(260).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(128).IsRequired();
        builder.Property(x => x.ContentBase64).IsRequired().HasColumnType("text");
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.JobApplication)
            .WithMany(a => a.Attachments)
            .HasForeignKey(x => x.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.IsActive, x.IsDeleted });
        builder.HasIndex(x => x.UserId);
    }
}
