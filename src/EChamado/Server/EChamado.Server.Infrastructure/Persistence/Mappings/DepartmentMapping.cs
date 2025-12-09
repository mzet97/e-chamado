using EChamado.Server.Domain.Domains.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EChamado.Server.Infrastructure.Persistence.Mappings
{
    public class DepartmentMapping : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Department");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired(true)
                .HasColumnType("varchar")
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasColumnType("varchar")
                .HasMaxLength(500);

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.Property(x => x.UpdatedAtUtc);

            builder.Property(x => x.DeletedAtUtc);

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            // Indexes for Gridify query performance
            builder.HasIndex(x => x.Name)
                .HasDatabaseName("IX_Department_Name");

            builder.HasIndex(x => x.CreatedAtUtc)
                .HasDatabaseName("IX_Department_CreatedAtUtc");

            builder.HasIndex(x => x.IsDeleted)
                .HasDatabaseName("IX_Department_IsDeleted");

            // Composite index for common filtering scenario
            builder.HasIndex(x => new { x.IsDeleted, x.Name })
                .HasDatabaseName("IX_Department_IsDeleted_Name");
        }
    }
}
