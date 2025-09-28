using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.Interfaces;

public interface ITeamCategoryRepository
{
    Task<TeamCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TeamCategory?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<TeamCategory>> GetActiveBySportAsync(Sport sport, CancellationToken cancellationToken = default);
    Task<List<TeamCategory>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsWithCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(TeamCategory teamCategory, CancellationToken cancellationToken = default);
    Task UpdateAsync(TeamCategory teamCategory, CancellationToken cancellationToken = default);
    Task DeleteAsync(TeamCategory teamCategory, CancellationToken cancellationToken = default);
}