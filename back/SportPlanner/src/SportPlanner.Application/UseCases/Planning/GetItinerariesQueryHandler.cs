using MediatR;
using SportPlanner.Application.Dtos.Planning;
using SportPlanner.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SportPlanner.Application.UseCases.Planning;

public class GetItinerariesQueryHandler : IRequestHandler<GetItinerariesQuery, IReadOnlyCollection<ItineraryDto>>
{
    private readonly IItineraryRepository _itineraryRepository;

    public GetItinerariesQueryHandler(IItineraryRepository itineraryRepository)
    {
        _itineraryRepository = itineraryRepository;
    }

    public async Task<IReadOnlyCollection<ItineraryDto>> Handle(GetItinerariesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var itineraries = await _itineraryRepository.GetAllWithItemsAsync(cancellationToken);

            var itineraryDtos = itineraries
                .Where(i => i.IsActive)
                .Select(i => new ItineraryDto(
                    i.Id,
                    i.Name,
                    i.Description,
                    i.Sport,
                    i.Level,
                    i.IsActive,
                    i.Items
                        .OrderBy(item => item.Order)
                        .Select(item => new ItineraryItemDto(
                            item.MarketplaceItemId,
                            item.MarketplaceItem.Name, // Assumes MarketplaceItem is loaded
                            item.MarketplaceItem.Type,
                            item.Order))
                        .ToList()))
                .ToList();

            return itineraryDtos;

        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
       
    }
}