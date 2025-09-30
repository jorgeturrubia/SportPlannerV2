using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Application.Interfaces;

public interface IWorkoutRepository
{
    Task<Workout?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Workout>> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
    Task<List<WorkoutDto>> GetWorkoutsDtoBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
    Task<List<Workout>> GetByPlanIdAsync(Guid trainingPlanId, CancellationToken cancellationToken = default);
    Task AddAsync(Workout workout, CancellationToken cancellationToken = default);
    Task UpdateAsync(Workout workout, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}