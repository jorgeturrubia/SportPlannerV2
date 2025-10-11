using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.Dtos.Planning;

public class MarketplaceSearchDto
{
    public Sport Sport { get; set; }
    public MarketplaceItemType? Type { get; set; }
    public MarketplaceFilter Filter { get; set; } = MarketplaceFilter.All;
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}


public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}