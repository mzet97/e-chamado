using EChamado.Server.Domain.Domains.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EChamado.Server.Infrastructure.Persistence.Mappings;

public class AttachmentMapping : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachment");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.SizeBytes)
            .IsRequired();

        builder.Property(a => a.StoragePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.CommentId)
            .IsRequired();

        builder.Property(a => a.UploadedByUserId)
            .IsRequired();

        builder.Property(a => a.UploadedByUserEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasOne(a => a.Comment)
            .WithMany()
            .HasForeignKey(a => a.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(a => a.CreatedAtUtc).IsRequired();
        builder.Property(a => a.UpdatedAtUtc);
        builder.Property(a => a.DeletedAtUtc);
        builder.Property(a => a.IsDeleted).IsRequired();

        builder.HasIndex(a => a.CommentId)
            .HasDatabaseName("IX_Attachment_CommentId");

        builder.HasIndex(a => a.IsDeleted)
            .HasDatabaseName("IX_Attachment_IsDeleted");
    }
}
