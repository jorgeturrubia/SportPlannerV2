using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories;

public class SportRepository : ISportRepository
{
    private readonly SportPlannerDbContext _context;

    public SportRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<Sport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sports
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sport?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Sports
            .FirstOrDefaultAsync(s => s.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<List<Sport>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sports
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Sport>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sports
            .Where(s => s.IsActive)
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sports
            .AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task AddAsync(Sport sport, CancellationToken cancellationToken = default)
    {
        await _context.Sports.AddAsync(sport, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Sport sport, CancellationToken cancellationToken = default)
    {
        _context.Sports.Update(sport);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
