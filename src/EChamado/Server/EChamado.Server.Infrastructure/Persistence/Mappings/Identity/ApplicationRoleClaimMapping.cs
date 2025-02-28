using EChamado.Server.Domain.Domains.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EChamado.Server.Infrastructure.Persistence.Mappings.Identity
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
