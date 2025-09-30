using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SportPlanner.Infrastructure.Data;

/// <summary>
/// Factory for creating DbContext instances at design time (for migrations).
/// </summary>
public class SportPlannerDbContextFactory : IDesignTimeDbContextFactory<SportPlannerDbContext>
{
    public SportPlannerDbContext CreateDbContext(string[] args)
    {
        // Use a dummy connection string for design-time (migrations)
        // The actual connection string will be used at runtime from appsettings
        var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")
            ?? "Host=localhost;Database=sportplanner;Username=postgres;Password=postgres";

        // Build DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<SportPlannerDbContext>();
        optionsBuilder.UseNpgsql(connectionString,
            b => b.MigrationsAssembly("SportPlanner.Infrastructure"));

        return new SportPlannerDbContext(optionsBuilder.Options);
    }
}