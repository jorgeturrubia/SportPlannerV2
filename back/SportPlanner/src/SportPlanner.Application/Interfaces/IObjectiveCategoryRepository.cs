using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Application.Interfaces;

public interface IObjectiveCategoryRepository
{
    Task<ObjectiveCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ObjectiveCategory>> GetBySportAsync(Guid sportId, CancellationToken cancellationToken = default);
    Task<List<ObjectiveCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ObjectiveCategory category, CancellationToken cancellationToken = default);
    Task UpdateAsync(ObjectiveCategory category, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}