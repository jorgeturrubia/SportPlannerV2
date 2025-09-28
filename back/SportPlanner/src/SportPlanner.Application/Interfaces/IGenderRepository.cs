using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.Interfaces;

public interface IGenderRepository
{
    Task<Gender?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Gender?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<Gender>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsWithCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(Gender gender, CancellationToken cancellationToken = default);
    Task UpdateAsync(Gender gender, CancellationToken cancellationToken = default);
    Task DeleteAsync(Gender gender, CancellationToken cancellationToken = default);
}