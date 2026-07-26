using EChamado.Server.Domain.Domains.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EChamado.Server.Infrastructure.Persistence.Mappings;

public class SlaPolicyMapping : IEntityTypeConfiguration<SlaPolicy>
{
    public void Configure(EntityTypeBuilder<SlaPolicy> builder)
    {
        builder.ToTable("SlaPolicy");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.CategoryId);
        builder.Property(s => s.DepartmentId);

        builder.Property(s => s.ResponseHours).IsRequired();
        builder.Property(s => s.ResolutionHours).IsRequired();
        builder.Property(s => s.Priority).IsRequired();

        builder.HasOne(s => s.Category)
            .WithMany()
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Department)
            .WithMany()
            .HasForeignKey(s => s.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc);
        builder.Property(s => s.DeletedAtUtc);
        builder.Property(s => s.IsDeleted).IsRequired();

        builder.HasIndex(s => s.CategoryId).HasDatabaseName("IX_SlaPolicy_CategoryId");
        builder.HasIndex(s => s.DepartmentId).HasDatabaseName("IX_SlaPolicy_DepartmentId");
        builder.HasIndex(s => s.Priority).HasDatabaseName("IX_SlaPolicy_Priority");
        builder.HasIndex(s => s.IsDeleted).HasDatabaseName("IX_SlaPolicy_IsDeleted");
    }
}
