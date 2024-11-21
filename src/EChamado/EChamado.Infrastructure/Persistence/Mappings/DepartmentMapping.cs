using EChamado.Core.Domains.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EChamado.Infrastructure.Persistence.Mappings
{
    public class DepartmentMapping : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Department");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired(true)
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasColumnType("NVARCHAR")
                .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
            .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.Property(x => x.DeletedAt);

            builder.Property(x => x.IsDeleted)
                .IsRequired();
        }
    }
}
