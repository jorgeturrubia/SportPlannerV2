using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportPlanner.Application.Interfaces;
using SportPlanner.Infrastructure.Data;
using SportPlanner.Infrastructure.ExternalServices;
using SportPlanner.Infrastructure.Repositories;
using SportPlanner.Infrastructure.Repositories.Planning;

namespace SportPlanner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<SportPlannerDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("SportPlanner.Infrastructure"))
            .UseSnakeCaseNamingConvention());

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<ISubscriptionUserRepository, SubscriptionUserRepository>();
        services.AddScoped<ITeamCategoryRepository, TeamCategoryRepository>();
        services.AddScoped<IGenderRepository, GenderRepository>();
        services.AddScoped<IAgeGroupRepository, AgeGroupRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();

        // Planning Repositories - Objectives
        services.AddScoped<IObjectiveRepository, ObjectiveRepository>();
        services.AddScoped<IObjectiveCategoryRepository, ObjectiveCategoryRepository>();
        services.AddScoped<IObjectiveSubcategoryRepository, ObjectiveSubcategoryRepository>();

        // Planning Repositories - Plans
        services.AddScoped<ITrainingPlanRepository, TrainingPlanRepository>();

        // Planning Repositories - Exercises
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IExerciseCategoryRepository, ExerciseCategoryRepository>();
        services.AddScoped<IExerciseTypeRepository, ExerciseTypeRepository>();

        // Planning Repositories - Workouts
        services.AddScoped<IWorkoutRepository, WorkoutRepository>();

        // Planning Repositories - Marketplace
        services.AddScoped<IMarketplaceItemRepository, MarketplaceItemRepository>();

        // External Services
        services.AddScoped<IAuthService, SupabaseAuthService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
