using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Application.Interfaces;

public interface IExerciseTypeRepository
{
    Task<ExerciseType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ExerciseType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}