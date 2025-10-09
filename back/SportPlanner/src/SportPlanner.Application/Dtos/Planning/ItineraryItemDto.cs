using SportPlanner.Domain.Enum;
using System;

namespace SportPlanner.Application.Dtos.Planning;

/// <summary>
/// Represents a single item within an itinerary, providing a summary of the marketplace item.
/// </summary>
public record ItineraryItemDto(
    Guid MarketplaceItemId,
    string Name,
    MarketplaceItemType Type,
    int Order);