using EChamado.Server.Domain.Domains.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EChamado.Server.Infrastructure.Persistence.Mappings;

public class OrderMapping : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Evaluation)
            .HasMaxLength(1000);

        builder.Property(o => o.OpeningDate)
            .IsRequired();

        builder.Property(o => o.ClosingDate);

        builder.Property(o => o.DueDate);

        builder.Property(o => o.StatusId)
            .IsRequired();

        builder.HasOne(o => o.Status)
            .WithMany()
            .HasForeignKey(o => o.StatusId);

        builder.Property(o => o.TypeId)
            .IsRequired();

        builder.HasOne(o => o.Type)
            .WithMany()
            .HasForeignKey(o => o.TypeId);

        builder.Property(o => o.RequestingUserId)
            .IsRequired();

        builder.Property(o => o.RequestingUserEmail)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.ResponsibleUserId)
            .IsRequired();

        builder.Property(o => o.ResponsibleUserEmail)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.CategoryId)
            .IsRequired();

        builder.HasOne(o => o.Category)
            .WithMany()
            .HasForeignKey(o => o.CategoryId);

        builder.Property(o => o.SubCategoryId);

        builder.HasOne(o => o.SubCategory)
            .WithMany()
            .HasForeignKey(o => o.SubCategoryId);

        builder.Property(o => o.DepartmentId)
            .IsRequired();

        builder.HasOne(o => o.Department)
            .WithMany()
            .HasForeignKey(o => o.DepartmentId);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt);

        builder.Property(o => o.DeletedAt);

        builder.Property(o => o.IsDeleted)
            .IsRequired();

        // Indexes for Gridify query performance
        builder.HasIndex(o => o.StatusId)
            .HasDatabaseName("IX_Order_StatusId");

        builder.HasIndex(o => o.TypeId)
            .HasDatabaseName("IX_Order_TypeId");

        builder.HasIndex(o => o.CategoryId)
            .HasDatabaseName("IX_Order_CategoryId");

        builder.HasIndex(o => o.SubCategoryId)
            .HasDatabaseName("IX_Order_SubCategoryId");

        builder.HasIndex(o => o.DepartmentId)
            .HasDatabaseName("IX_Order_DepartmentId");

        builder.HasIndex(o => o.RequestingUserId)
            .HasDatabaseName("IX_Order_RequestingUserId");

        builder.HasIndex(o => o.ResponsibleUserId)
            .HasDatabaseName("IX_Order_ResponsibleUserId");

        builder.HasIndex(o => o.OpeningDate)
            .HasDatabaseName("IX_Order_OpeningDate");

        builder.HasIndex(o => o.ClosingDate)
            .HasDatabaseName("IX_Order_ClosingDate");

        builder.HasIndex(o => o.DueDate)
            .HasDatabaseName("IX_Order_DueDate");

        builder.HasIndex(o => o.CreatedAt)
            .HasDatabaseName("IX_Order_CreatedAt");

        builder.HasIndex(o => o.IsDeleted)
            .HasDatabaseName("IX_Order_IsDeleted");

        // Composite index for common filtering scenario
        builder.HasIndex(o => new { o.IsDeleted, o.StatusId, o.CreatedAt })
            .HasDatabaseName("IX_Order_IsDeleted_StatusId_CreatedAt");

        builder.ToTable("Order");
    }
}
