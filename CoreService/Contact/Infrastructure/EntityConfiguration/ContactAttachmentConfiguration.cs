using CoreService.Auth.Domain.Entities;
using CoreService.Contact.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreService.Contact.Infrastructure.EntityConfiguration;

public class ContactAttachmentConfiguration : IEntityTypeConfiguration<ContactAttachment>
{
    public void Configure(EntityTypeBuilder<ContactAttachment> builder)
    {
        builder.ToTable("contact_attachments");
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

        builder.HasOne(x => x.ContactMessage)
            .WithMany(m => m.Attachments)
            .HasForeignKey(x => x.ContactMessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.IsActive, x.IsDeleted });
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ContactMessageId);
        builder.HasIndex(x => new { x.ContactMessageId, x.CreatedAtUtc });
    }
}
