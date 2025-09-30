using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Infrastructure.Configurations;

public class ObjectiveCategoryConfiguration : IEntityTypeConfiguration<ObjectiveCategory>
{
    public void Configure(EntityTypeBuilder<ObjectiveCategory> builder)
    {
        builder.HasKey(oc => oc.Id);

        builder.Property(oc => oc.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(oc => oc.Sport)
            .IsRequired()
            .HasConversion<string>();

        // Audit properties
        builder.Property(oc => oc.CreatedAt)
            .IsRequired();

        builder.Property(oc => oc.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(oc => oc.UpdatedBy)
            .HasMaxLength(255);

        // Indices
        builder.HasIndex(oc => oc.Sport)
            .HasDatabaseName("IX_ObjectiveCategories_Sport");

        builder.HasIndex(oc => new { oc.Sport, oc.Name })
            .IsUnique()
            .HasDatabaseName("IX_ObjectiveCategories_Sport_Name");

        // Table name
        builder.ToTable("objective_categories");
    }
}