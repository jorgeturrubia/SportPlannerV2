using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities;

namespace SportPlanner.Infrastructure.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);

        // Basic properties
        builder.Property(t => t.SubscriptionId)
            .IsRequired();

        builder.Property(t => t.TeamCategoryId)
            .IsRequired();

        builder.Property(t => t.GenderId)
            .IsRequired();

        builder.Property(t => t.AgeGroupId)
            .IsRequired();

        builder.Property(t => t.CoachSubscriptionUserId);

        builder.Property(t => t.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Color)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.Sport)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.IsActive)
            .IsRequired();

        // Additional properties
        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.MaxPlayers)
            .IsRequired();

        builder.Property(t => t.CurrentPlayersCount)
            .IsRequired();

        builder.Property(t => t.AllowMixedGender)
            .IsRequired();

        // Audit properties
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.UpdatedBy)
            .HasMaxLength(255);

        // Relationships
        builder.HasOne<Subscription>()
            .WithMany()
            .HasForeignKey(t => t.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Category)
            .WithMany()
            .HasForeignKey(t => t.TeamCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Gender)
            .WithMany()
            .HasForeignKey(t => t.GenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.AgeGroup)
            .WithMany()
            .HasForeignKey(t => t.AgeGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Coach)
            .WithMany()
            .HasForeignKey(t => t.CoachSubscriptionUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indices
        builder.HasIndex(t => new { t.SubscriptionId, t.Name })
            .IsUnique()
            .HasDatabaseName("IX_Teams_SubscriptionId_Name");

        builder.HasIndex(t => t.SubscriptionId)
            .HasDatabaseName("IX_Teams_SubscriptionId");

        builder.HasIndex(t => t.TeamCategoryId)
            .HasDatabaseName("IX_Teams_TeamCategoryId");

        builder.HasIndex(t => t.GenderId)
            .HasDatabaseName("IX_Teams_GenderId");

        builder.HasIndex(t => t.AgeGroupId)
            .HasDatabaseName("IX_Teams_AgeGroupId");

        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("IX_Teams_IsActive");

        builder.HasIndex(t => new { t.SubscriptionId, t.IsActive })
            .HasDatabaseName("IX_Teams_SubscriptionId_IsActive");

        // Table name
        builder.ToTable("teams");
    }
}