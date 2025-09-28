using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.Interfaces;

public interface IAgeGroupRepository
{
    Task<AgeGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AgeGroup?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<AgeGroup>> GetActiveBySportAsync(Sport sport, CancellationToken cancellationToken = default);
    Task<List<AgeGroup>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<AgeGroup?> GetByAgeAndSportAsync(int age, Sport sport, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(AgeGroup ageGroup, CancellationToken cancellationToken = default);
    Task UpdateAsync(AgeGroup ageGroup, CancellationToken cancellationToken = default);
    Task DeleteAsync(AgeGroup ageGroup, CancellationToken cancellationToken = default);
}