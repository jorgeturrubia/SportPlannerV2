using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.Interfaces;

public interface ISportRepository
{
    Task<Sport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sport?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<Sport>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Sport>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Sport sport, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sport sport, CancellationToken cancellationToken = default);
}
