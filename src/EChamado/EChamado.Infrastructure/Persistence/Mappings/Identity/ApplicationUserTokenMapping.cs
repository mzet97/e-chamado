using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EChamado.Core.Domains.Identities;

namespace EChamado.Infrastructure.Persistence.Mappings.Identity
{
    public class ApplicationUserTokenMapping : IEntityTypeConfiguration<ApplicationUserToken>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserToken> builder)
        {
            builder.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

            builder.ToTable("AspNetUserTokens");
        }
    }
}
