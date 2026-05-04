using CoreService.Contact.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreService.Contact.Infrastructure;

public class ContactAttachmentConfiguration : IEntityTypeConfiguration<ContactAttachment>
{
    public void Configure(EntityTypeBuilder<ContactAttachment> builder)
    {
        builder.ToTable("contact_attachments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OriginalFileName).HasMaxLength(260).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(128).IsRequired();
        builder.Property(x => x.ContentBase64).IsRequired().HasColumnType("text");
        builder.HasOne(x => x.ContactMessage)
            .WithMany(m => m.Attachments)
            .HasForeignKey(x => x.ContactMessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
