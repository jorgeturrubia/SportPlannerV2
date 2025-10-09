using SportPlanner.Domain.Entities.Planning;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SportPlanner.Application.Interfaces;

public interface IItineraryRepository
{
    Task<Itinerary?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Itinerary>> GetAllWithItemsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Itinerary itinerary, CancellationToken cancellationToken = default);
    Task UpdateAsync(Itinerary itinerary, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}