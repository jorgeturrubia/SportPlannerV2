using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Application.Interfaces;

public interface IExerciseCategoryRepository
{
    Task<ExerciseCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ExerciseCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}