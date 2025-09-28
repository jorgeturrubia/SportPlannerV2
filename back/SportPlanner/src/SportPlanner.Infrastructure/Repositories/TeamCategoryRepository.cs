using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories;

public class TeamCategoryRepository : ITeamCategoryRepository
{
    private readonly SportPlannerDbContext _context;

    public TeamCategoryRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<TeamCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TeamCategories
            .FirstOrDefaultAsync(tc => tc.Id == id, cancellationToken);
    }

    public async Task<TeamCategory?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.TeamCategories
            .FirstOrDefaultAsync(tc => tc.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<List<TeamCategory>> GetActiveBySportAsync(Sport sport, CancellationToken cancellationToken = default)
    {
        return await _context.TeamCategories
            .Where(tc => tc.Sport == sport && tc.IsActive)
            .OrderBy(tc => tc.SortOrder)
            .ThenBy(tc => tc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TeamCategory>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TeamCategories
            .Where(tc => tc.IsActive)
            .OrderBy(tc => tc.Sport)
            .ThenBy(tc => tc.SortOrder)
            .ThenBy(tc => tc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsWithCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.TeamCategories
            .AnyAsync(tc => tc.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task AddAsync(TeamCategory teamCategory, CancellationToken cancellationToken = default)
    {
        await _context.TeamCategories.AddAsync(teamCategory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TeamCategory teamCategory, CancellationToken cancellationToken = default)
    {
        _context.TeamCategories.Update(teamCategory);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TeamCategory teamCategory, CancellationToken cancellationToken = default)
    {
        _context.TeamCategories.Remove(teamCategory);
        await _context.SaveChangesAsync(cancellationToken);
    }
}