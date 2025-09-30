using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SportPlanner.Domain.Services;
using System.Reflection;

namespace SportPlanner.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Domain Services
        services.AddScoped<WorkoutAutoGeneratorService>();

        return services;
    }
}
