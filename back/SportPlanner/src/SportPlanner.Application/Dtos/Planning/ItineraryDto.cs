using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using System;
using System.Collections.Generic;

namespace SportPlanner.Application.Dtos.Planning;

/// <summary>
/// Data transfer object for a full itinerary, including its list of items.
/// </summary>
public record ItineraryDto(
    Guid Id,
    string Name,
    string Description,
    Sport Sport,
    Difficulty Level,
    bool IsActive,
    IReadOnlyCollection<ItineraryItemDto> Items);