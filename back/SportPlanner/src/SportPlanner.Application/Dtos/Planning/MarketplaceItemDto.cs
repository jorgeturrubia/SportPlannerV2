using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using System;
using System.Collections.Generic;

namespace SportPlanner.Application.Dtos.Planning;

/// <summary>
/// Data transfer object for a marketplace item.
/// </summary>
public record MarketplaceItemDto(
    Guid Id,
    MarketplaceItemType Type,
    Sport Sport,
    Guid? SourceEntityId,
    string Name,
    string Description,
    bool IsSystemOfficial,
    decimal AverageRating,
    int TotalRatings,
    int TotalDownloads,
    int TotalViews,
    DateTime PublishedAt,
    IReadOnlyCollection<MarketplaceRatingDto> Ratings);