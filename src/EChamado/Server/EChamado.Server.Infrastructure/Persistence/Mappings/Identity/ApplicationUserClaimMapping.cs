using EChamado.Server.Domain.Domains.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EChamado.Server.Infrastructure.Persistence.Mappings.Identity
{
    public class ApplicationUserClaimMapping : IEntityTypeConfiguration<ApplicationUserClaim>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
        {
            builder.HasKey(uc => uc.Id);

            builder.ToTable("AspNetUserClaims");
        }
    }
}
