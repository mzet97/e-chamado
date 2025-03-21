﻿using EChamado.Server.Domain.Domains.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EChamado.Server.Infrastructure.Persistence.Mappings.Identity
{
    public class ApplicationUserLoginMapping : IEntityTypeConfiguration<ApplicationUserLogin>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserLogin> builder)
        {
            builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

            builder.ToTable("AspNetUserLogins");
        }
    }
}
