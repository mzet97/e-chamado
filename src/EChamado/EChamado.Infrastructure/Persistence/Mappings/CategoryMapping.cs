﻿using EChamado.Core.Domains.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EChamado.Infrastructure.Persistence.Mappings;

public class CategoryMapping : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired(true)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(500);

        builder.HasMany(x => x.SubCategories)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.DeletedAt);

        builder.Property(x => x.IsDeleted)
            .IsRequired();
    }
}