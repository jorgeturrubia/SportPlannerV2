using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Application.Interfaces;

public interface IExerciseRepository
{
    Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Exercise>> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
    Task<List<ExerciseDto>> GetExercisesDtoBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
    Task<List<Exercise>> GetSystemExercisesAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Exercise exercise, CancellationToken cancellationToken = default);
    Task UpdateAsync(Exercise exercise, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}