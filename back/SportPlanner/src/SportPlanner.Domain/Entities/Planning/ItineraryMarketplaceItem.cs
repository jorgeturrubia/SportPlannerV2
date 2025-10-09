namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Joining entity to represent the many-to-many relationship
/// between Itinerary and MarketplaceItem.
/// </summary>
public class ItineraryMarketplaceItem
{
    public Guid ItineraryId { get; private set; }
    public Itinerary Itinerary { get; private set; } = null!;

    public Guid MarketplaceItemId { get; private set; }
    public MarketplaceItem MarketplaceItem { get; private set; } = null!;

    /// <summary>
    /// The order of the item within the itinerary.
    /// </summary>
    public int Order { get; private set; }

    // For EF Core
    private ItineraryMarketplaceItem() { }

    public ItineraryMarketplaceItem(Guid itineraryId, Guid marketplaceItemId, int order)
    {
        if (itineraryId == Guid.Empty)
            throw new ArgumentException("Itinerary ID cannot be empty.", nameof(itineraryId));

        if (marketplaceItemId == Guid.Empty)
            throw new ArgumentException("Marketplace Item ID cannot be empty.", nameof(marketplaceItemId));

        if (order < 0)
            throw new ArgumentException("Order must be a non-negative integer.", nameof(order));

        ItineraryId = itineraryId;
        MarketplaceItemId = marketplaceItemId;
        Order = order;
    }
}