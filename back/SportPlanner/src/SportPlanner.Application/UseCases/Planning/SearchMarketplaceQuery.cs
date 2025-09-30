using MediatR;
using SportPlanner.Application.DTOs.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public record SearchMarketplaceQuery(MarketplaceSearchDto SearchCriteria) : IRequest<PagedResult<MarketplaceItemDto>>;