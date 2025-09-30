namespace SportPlanner.Application.DTOs.Planning;

public class ObjectiveSubcategoryDto
{
    public Guid Id { get; set; }
    public Guid ObjectiveCategoryId { get; set; }
    public string Name { get; set; }
}