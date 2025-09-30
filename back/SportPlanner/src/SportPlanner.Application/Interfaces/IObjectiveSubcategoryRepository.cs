using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Application.Interfaces;

public interface IObjectiveSubcategoryRepository
{
    Task<ObjectiveSubcategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ObjectiveSubcategory>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<List<ObjectiveSubcategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}