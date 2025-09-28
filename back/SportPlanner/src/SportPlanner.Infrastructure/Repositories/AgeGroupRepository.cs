using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories;

public class AgeGroupRepository : IAgeGroupRepository
{
    private readonly SportPlannerDbContext _context;

    public AgeGroupRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<AgeGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AgeGroups
            .FirstOrDefaultAsync(ag => ag.Id == id, cancellationToken);
    }

    public async Task<AgeGroup?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.AgeGroups
            .FirstOrDefaultAsync(ag => ag.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<List<AgeGroup>> GetActiveBySportAsync(Sport sport, CancellationToken cancellationToken = default)
    {
        return await _context.AgeGroups
            .Where(ag => ag.Sport == sport && ag.IsActive)
            .OrderBy(ag => ag.SortOrder)
            .ThenBy(ag => ag.MinAge)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AgeGroup>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AgeGroups
            .Where(ag => ag.IsActive)
            .OrderBy(ag => ag.Sport)
            .ThenBy(ag => ag.SortOrder)
            .ThenBy(ag => ag.MinAge)
            .ToListAsync(cancellationToken);
    }

    public async Task<AgeGroup?> GetByAgeAndSportAsync(int age, Sport sport, CancellationToken cancellationToken = default)
    {
        return await _context.AgeGroups
            .Where(ag => ag.Sport == sport && ag.IsActive)
            .FirstOrDefaultAsync(ag => age >= ag.MinAge && age <= ag.MaxAge, cancellationToken);
    }

    public async Task<bool> ExistsWithCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.AgeGroups
            .AnyAsync(ag => ag.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task AddAsync(AgeGroup ageGroup, CancellationToken cancellationToken = default)
    {
        await _context.AgeGroups.AddAsync(ageGroup, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AgeGroup ageGroup, CancellationToken cancellationToken = default)
    {
        _context.AgeGroups.Update(ageGroup);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(AgeGroup ageGroup, CancellationToken cancellationToken = default)
    {
        _context.AgeGroups.Remove(ageGroup);
        await _context.SaveChangesAsync(cancellationToken);
    }
}