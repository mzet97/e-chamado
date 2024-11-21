using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EChamado.Core.Domains.Identities;

namespace EChamado.Infrastructure.Persistence.Mappings.Identity
{
    public class ApplicationRoleClaimMapping : IEntityTypeConfiguration<ApplicationRoleClaim>
    {
        public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
        {
            builder.HasKey(rc => rc.Id);

            builder.ToTable("AspNetRoleClaims");
        }
    }
}
