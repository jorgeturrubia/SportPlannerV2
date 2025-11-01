using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Application.Interfaces;

public interface IExerciseTypeRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}