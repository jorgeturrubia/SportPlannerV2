namespace SportPlanner.Application.DTOs.Planning;

public class ObjectiveCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public SportDto Sport { get; set; } = null!;
}