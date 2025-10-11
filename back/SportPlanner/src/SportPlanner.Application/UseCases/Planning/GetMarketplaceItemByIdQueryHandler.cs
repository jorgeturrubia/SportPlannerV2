using MediatR;
using SportPlanner.Application.Dtos.Planning;
using SportPlanner.Application.Interfaces;
using SportPlanner.Shared.Exceptions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SportPlanner.Application.UseCases.Planning;

public class GetMarketplaceItemByIdQueryHandler : IRequestHandler<GetMarketplaceItemByIdQuery, MarketplaceItemDto>
{
    private readonly IMarketplaceItemRepository _marketplaceItemRepository;

    public GetMarketplaceItemByIdQueryHandler(IMarketplaceItemRepository marketplaceItemRepository)
    {
        _marketplaceItemRepository = marketplaceItemRepository;
    }

    public async Task<MarketplaceItemDto> Handle(GetMarketplaceItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _marketplaceItemRepository.GetByIdWithRatingsAsync(request.MarketplaceItemId, cancellationToken)
            ?? throw new NotFoundException($"MarketplaceItem with ID '{request.MarketplaceItemId}' not found.");

        var ratingDtos = item.Ratings
            .Select(r => new MarketplaceRatingDto(r.RatedBySubscriptionId, r.Stars, r.Comment, r.CreatedAt))
            .ToList();

        return new MarketplaceItemDto(
            item.Id,
            item.Type,
            item.Sport,
            item.SourceEntityId,
            item.Name,
            item.Description,
            item.IsSystemOfficial,
            item.AverageRating,
            item.TotalRatings,
            item.TotalDownloads,
            item.TotalViews,
            item.PublishedAt,
            ratingDtos);
    }
}