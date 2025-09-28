using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportPlanner.Application.Interfaces;
using SportPlanner.Infrastructure.Data;
using SportPlanner.Infrastructure.ExternalServices;
using SportPlanner.Infrastructure.Repositories;

namespace SportPlanner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<SportPlannerDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("SportPlanner.Infrastructure")));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<ISubscriptionUserRepository, SubscriptionUserRepository>();

        // External Services
        services.AddScoped<IAuthService, SupabaseAuthService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
