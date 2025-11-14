using EChamado.Server.Domain.Domains.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EChamado.Server.Infrastructure.Persistence.Mappings;

public class CommentMapping : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comment");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text)
            .IsRequired(true)
            .HasColumnType("varchar")
            .HasMaxLength(2000);

        builder.Property(x => x.OrderId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.UserEmail)
            .IsRequired(true)
            .HasColumnType("varchar")
            .HasMaxLength(256);

        builder.HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.DeletedAt);

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.HasIndex(x => x.OrderId);
    }
}
