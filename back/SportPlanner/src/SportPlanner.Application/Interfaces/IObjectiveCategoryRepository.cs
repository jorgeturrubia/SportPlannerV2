using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.Interfaces;

public interface IObjectiveCategoryRepository
{
    Task<ObjectiveCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ObjectiveCategory>> GetBySportAsync(Sport sport, CancellationToken cancellationToken = default);
    Task<List<ObjectiveCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}