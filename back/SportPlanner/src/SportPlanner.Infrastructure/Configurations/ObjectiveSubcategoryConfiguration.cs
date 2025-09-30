using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Infrastructure.Configurations;

public class ObjectiveSubcategoryConfiguration : IEntityTypeConfiguration<ObjectiveSubcategory>
{
    public void Configure(EntityTypeBuilder<ObjectiveSubcategory> builder)
    {
        builder.HasKey(osc => osc.Id);

        builder.Property(osc => osc.ObjectiveCategoryId)
            .IsRequired();

        builder.Property(osc => osc.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Audit properties
        builder.Property(osc => osc.CreatedAt)
            .IsRequired();

        builder.Property(osc => osc.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(osc => osc.UpdatedBy)
            .HasMaxLength(255);

        // Relationships
        builder.HasOne(osc => osc.Category)
            .WithMany()
            .HasForeignKey(osc => osc.ObjectiveCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indices
        builder.HasIndex(osc => osc.ObjectiveCategoryId)
            .HasDatabaseName("IX_ObjectiveSubcategories_ObjectiveCategoryId");

        builder.HasIndex(osc => new { osc.ObjectiveCategoryId, osc.Name })
            .IsUnique()
            .HasDatabaseName("IX_ObjectiveSubcategories_CategoryId_Name");

        // Table name
        builder.ToTable("objective_subcategories");
    }
}