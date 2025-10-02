using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SportPlanner.Infrastructure.Data;

/// <summary>
/// Factory for creating DbContext instances at design time (for migrations).
/// </summary>
public class SportPlannerDbContextFactory : IDesignTimeDbContextFactory<SportPlannerDbContext>
{
    public SportPlannerDbContext CreateDbContext(string[] args)
    {
        // Build configuration to read from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../SportPlanner.API"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .Build();

        // Get connection string from configuration or environment variable
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("DefaultConnection")
            ?? "Host=localhost;Database=sportplanner;Username=postgres;Password=postgres";

        // Build DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<SportPlannerDbContext>();
        optionsBuilder.UseNpgsql(connectionString,
            b => b.MigrationsAssembly("SportPlanner.Infrastructure"))
            .UseSnakeCaseNamingConvention();

        return new SportPlannerDbContext(optionsBuilder.Options);
    }
}