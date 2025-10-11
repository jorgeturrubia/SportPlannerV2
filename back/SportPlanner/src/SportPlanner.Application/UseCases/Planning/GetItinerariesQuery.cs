using MediatR;
using SportPlanner.Application.Dtos.Planning;
using System.Collections.Generic;

namespace SportPlanner.Application.UseCases.Planning;

/// <summary>
/// Query to fetch a list of all active itineraries.
/// </summary>
public record GetItinerariesQuery() : IRequest<IReadOnlyCollection<ItineraryDto>>;