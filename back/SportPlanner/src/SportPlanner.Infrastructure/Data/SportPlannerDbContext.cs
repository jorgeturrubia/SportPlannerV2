using Microsoft.EntityFrameworkCore;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;
using SportPlanner.Domain.Interfaces;
using SportPlanner.Domain.ValueObjects;

namespace SportPlanner.Infrastructure.Data;

public class SportPlannerDbContext : DbContext
{
    public SportPlannerDbContext(DbContextOptions<SportPlannerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<SubscriptionUser> SubscriptionUsers { get; set; }
    public DbSet<TeamCategory> TeamCategories { get; set; }
    public DbSet<Gender> Genders { get; set; }
    public DbSet<AgeGroup> AgeGroups { get; set; }
    public DbSet<Team> Teams { get; set; }

    // Planning entities - Objectives
    public DbSet<Objective> Objectives { get; set; }
    public DbSet<ObjectiveCategory> ObjectiveCategories { get; set; }
    public DbSet<ObjectiveSubcategory> ObjectiveSubcategories { get; set; }

    // Planning entities - Training Plans
    public DbSet<TrainingPlan> TrainingPlans { get; set; }
    public DbSet<PlanObjective> PlanObjectives { get; set; }

    // Planning entities - Exercises
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<ExerciseCategory> ExerciseCategories { get; set; }
    public DbSet<ExerciseType> ExerciseTypes { get; set; }
    public DbSet<ExerciseObjective> ExerciseObjectives { get; set; }

    // Planning entities - Workouts
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
    public DbSet<WorkoutObjective> WorkoutObjectives { get; set; }

    // Planning entities - Calendar
    public DbSet<CalendarEvent> CalendarEvents { get; set; }

    // Planning entities - Marketplace
    public DbSet<MarketplaceItem> MarketplaceItems { get; set; }
    public DbSet<MarketplaceRating> MarketplaceRatings { get; set; }

    // Planning entities - Itineraries
    public DbSet<Itinerary> Itineraries { get; set; }
    public DbSet<ItineraryMarketplaceItem> ItineraryMarketplaceItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SportPlannerDbContext).Assembly);

        // Seed master data
        MasterDataSeeder.SeedMasterData(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(50).IsRequired();
            entity.Property(u => u.SupabaseUserId).HasMaxLength(255); // Optional - synced from Supabase Auth
            entity.Property(u => u.Role).IsRequired();
            entity.Property(u => u.CreatedAt).IsRequired();
            entity.Property(u => u.CreatedBy).HasMaxLength(255).IsRequired();
            entity.Property(u => u.UpdatedBy).HasMaxLength(255);

            // Configure Email value object
            entity.Property(u => u.Email)
                .HasConversion(
                    email => email.Value,
                    value => Email.Create(value))
                .HasMaxLength(255)
                .IsRequired();

            // Create unique index on email
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Configure Subscription entity
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.OwnerId).IsRequired();
            entity.Property(s => s.Type).IsRequired();
            entity.Property(s => s.Sport).IsRequired();
            entity.Property(s => s.MaxUsers).IsRequired();
            entity.Property(s => s.MaxTeams).IsRequired();
            entity.Property(s => s.IsActive).IsRequired();
            entity.Property(s => s.CreatedAt).IsRequired();
            entity.Property(s => s.CreatedBy).HasMaxLength(255).IsRequired();
            entity.Property(s => s.UpdatedBy).HasMaxLength(255);

            // Unique constraint: one subscription per owner
            entity.HasIndex(s => s.OwnerId).IsUnique();
        });

        // Configure SubscriptionUser entity
        modelBuilder.Entity<SubscriptionUser>(entity =>
        {
            entity.HasKey(su => su.Id);
            entity.Property(su => su.SubscriptionId).IsRequired();
            entity.Property(su => su.UserId).IsRequired();
            entity.Property(su => su.RoleInSubscription).IsRequired();
            entity.Property(su => su.GrantedBy).IsRequired();
            entity.Property(su => su.GrantedAt).IsRequired();
            entity.Property(su => su.CreatedAt).IsRequired();
            entity.Property(su => su.CreatedBy).HasMaxLength(255).IsRequired();
            entity.Property(su => su.UpdatedBy).HasMaxLength(255);

            // Foreign keys
            entity.HasOne<Subscription>()
                .WithMany() // No navigation property for now
                .HasForeignKey(su => su.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: one subscription user record per user per subscription
            entity.HasIndex(su => new { su.SubscriptionId, su.UserId }).IsUnique();

            // Index for active users
            entity.HasIndex(su => new { su.SubscriptionId, su.RemovedAt })
                .HasFilter("removed_at IS NULL");
        });

        // Configure Itinerary entity
        modelBuilder.Entity<Itinerary>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Name).HasMaxLength(200).IsRequired();
            entity.Property(i => i.Description).HasMaxLength(2000).IsRequired();
            entity.Property(i => i.Sport).IsRequired().HasConversion<string>();
            entity.Property(i => i.Level).IsRequired().HasConversion<string>();
            entity.Property(i => i.IsActive).IsRequired();
            entity.Property(i => i.CreatedAt).IsRequired();
            entity.Property(i => i.CreatedBy).HasMaxLength(255).IsRequired();
            entity.Property(i => i.UpdatedBy).HasMaxLength(255);
        });

        // Configure ItineraryMarketplaceItem joining entity
        modelBuilder.Entity<ItineraryMarketplaceItem>(entity =>
        {
            // Composite primary key
            entity.HasKey(imi => new { imi.ItineraryId, imi.MarketplaceItemId });

            // Foreign key to Itinerary
            entity.HasOne(imi => imi.Itinerary)
                .WithMany(i => i.Items)
                .HasForeignKey(imi => imi.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to MarketplaceItem
            entity.HasOne(imi => imi.MarketplaceItem)
                .WithMany() // No navigation property back from MarketplaceItem
                .HasForeignKey(imi => imi.MarketplaceItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<IAuditable>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedBy = "System"; // Placeholder
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = "System"; // Placeholder
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
