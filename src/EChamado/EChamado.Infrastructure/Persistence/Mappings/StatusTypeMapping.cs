﻿using EChamado.Core.Domains.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EChamado.Infrastructure.Persistence.Mappings;

public class StatusTypeMapping : IEntityTypeConfiguration<StatusType>
{
    public void Configure(EntityTypeBuilder<StatusType> builder)
    {
        builder.ToTable("StatusType");

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