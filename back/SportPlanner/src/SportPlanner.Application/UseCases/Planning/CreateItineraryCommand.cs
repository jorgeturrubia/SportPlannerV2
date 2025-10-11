using MediatR;
using SportPlanner.Domain.Enum;
using System;
using System.Collections.Generic;

namespace SportPlanner.Application.UseCases.Planning;

public record ItineraryItemToAdd(Guid MarketplaceItemId, int Order);

public record CreateItineraryCommand(
    string Name,
    string Description,
    Sport Sport,
    Difficulty Level,
    List<ItineraryItemToAdd> Items) : IRequest<Guid>;