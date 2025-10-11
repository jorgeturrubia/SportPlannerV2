using SportPlanner.Domain.Enum;
using SportPlanner.Domain.Interfaces;

namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Represents a curated collection of marketplace items, forming a structured "pack" or "itinerary".
/// Itineraries are typically created by the system/admins.
/// </summary>
public class Itinerary : Entity, IAuditable
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Sport Sport { get; private set; }
    public Difficulty Level { get; private set; }
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    private readonly List<ItineraryMarketplaceItem> _items = new();
    public IReadOnlyCollection<ItineraryMarketplaceItem> Items => _items.AsReadOnly();

    // For EF Core
    private Itinerary()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    public Itinerary(string name, string description, Sport sport, Difficulty level, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (name.Length > 200)
            throw new ArgumentException("Name cannot exceed 200 characters.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));
        if (description.Length > 2000)
            throw new ArgumentException("Description cannot exceed 2000 characters.", nameof(description));

        Name = name;
        Description = description;
        Sport = sport;
        Level = level;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void Update(string name, string description, Sport sport, Difficulty level, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (name.Length > 200)
            throw new ArgumentException("Name cannot exceed 200 characters.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));
        if (description.Length > 2000)
            throw new ArgumentException("Description cannot exceed 2000 characters.", nameof(description));

        Name = name;
        Description = description;
        Sport = sport;
        Level = level;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void AddItem(Guid marketplaceItemId, int order)
    {
        if (_items.Any(i => i.MarketplaceItemId == marketplaceItemId))
        {
            throw new InvalidOperationException("This item is already in the itinerary.");
        }

        var item = new ItineraryMarketplaceItem(Id, marketplaceItemId, order);
        _items.Add(item);
    }

    public void RemoveItem(Guid marketplaceItemId)
    {
        var itemToRemove = _items.FirstOrDefault(i => i.MarketplaceItemId == marketplaceItemId);
        if (itemToRemove is null)
        {
            throw new InvalidOperationException("Item not found in this itinerary.");
        }
        _items.Remove(itemToRemove);
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}