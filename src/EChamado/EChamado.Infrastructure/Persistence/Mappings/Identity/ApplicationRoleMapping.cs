using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EChamado.Core.Domains.Identities;

namespace EChamado.Infrastructure.Persistence.Mappings.Identity
{
    public class ApplicationRoleMapping : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .HasMaxLength(256);

            builder.Property(r => r.NormalizedName)
                .HasMaxLength(256);

            builder.ToTable("AspNetRoles");

            // Relacionamento com UserRoles
            builder.HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // Relacionamento com RoleClaims
            builder.HasMany(r => r.RoleClaims)
                .WithOne()
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();
        }
    }
}
