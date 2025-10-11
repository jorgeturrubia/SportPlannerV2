using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SportPlanner.Application.UseCases.Planning;

public class CreateItineraryCommandHandler : IRequestHandler<CreateItineraryCommand, Guid>
{
    private readonly IItineraryRepository _itineraryRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateItineraryCommandHandler(IItineraryRepository itineraryRepository, ICurrentUserService currentUserService)
    {
        _itineraryRepository = itineraryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateItineraryCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId().ToString();
        if (string.IsNullOrEmpty(userId) || userId == Guid.Empty.ToString())
        {
            throw new UnauthorizedAccessException("Cannot create an itinerary without a valid user.");
        }

        var itinerary = new Itinerary(
            request.Name,
            request.Description,
            request.Sport,
            request.Level,
            userId);

        foreach (var item in request.Items)
        {
            itinerary.AddItem(item.MarketplaceItemId, item.Order);
        }

        await _itineraryRepository.AddAsync(itinerary, cancellationToken);

        return itinerary.Id;
    }
}