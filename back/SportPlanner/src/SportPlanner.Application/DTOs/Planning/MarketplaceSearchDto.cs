using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs.Planning;

public class MarketplaceSearchDto
{
    public Sport Sport { get; set; }
    public MarketplaceItemType? Type { get; set; }
    public MarketplaceFilter Filter { get; set; } = MarketplaceFilter.All;
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class MarketplaceItemDto
{
    public Guid Id { get; set; }
    public MarketplaceItemType Type { get; set; }
    public Sport Sport { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSystemOfficial { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public int TotalDownloads { get; set; }
    public int TotalViews { get; set; }
    public DateTime PublishedAt { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}