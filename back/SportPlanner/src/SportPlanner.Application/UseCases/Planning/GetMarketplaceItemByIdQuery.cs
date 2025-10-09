using MediatR;
using SportPlanner.Application.Dtos.Planning;
using System;

namespace SportPlanner.Application.UseCases.Planning;

/// <summary>
/// Query to fetch a single marketplace item by its ID.
/// </summary>
public record GetMarketplaceItemByIdQuery(Guid MarketplaceItemId) : IRequest<MarketplaceItemDto>;