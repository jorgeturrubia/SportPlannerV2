namespace SportPlanner.Application.DTOs.Planning;

public class CreateObjectiveDto
{
    public Guid SportId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid ObjectiveCategoryId { get; set; }
    public Guid? ObjectiveSubcategoryId { get; set; }
    public int Level { get; set; } = 1; // Default to level 1
    public List<ObjectiveTechniqueDto> Techniques { get; set; } = new();
}