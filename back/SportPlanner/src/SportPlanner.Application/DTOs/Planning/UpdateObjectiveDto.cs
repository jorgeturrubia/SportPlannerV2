namespace SportPlanner.Application.DTOs.Planning;

public class UpdateObjectiveDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid ObjectiveCategoryId { get; set; }
    public Guid? ObjectiveSubcategoryId { get; set; }
    public int? Level { get; set; } // Optional: null means no change
    public List<ObjectiveTechniqueDto> Techniques { get; set; } = new();
}