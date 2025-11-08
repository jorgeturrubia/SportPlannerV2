namespace SportPlanner.Application.DTOs.Planning;

public class CreateObjectiveCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public Guid SportId { get; set; }
}
