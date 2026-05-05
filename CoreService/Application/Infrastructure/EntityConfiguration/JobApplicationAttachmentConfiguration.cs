using CoreService.Application.Domain.Entities;
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
        builder.HasOne(x => x.JobApplication)
            .WithMany(a => a.Attachments)
            .HasForeignKey(x => x.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
