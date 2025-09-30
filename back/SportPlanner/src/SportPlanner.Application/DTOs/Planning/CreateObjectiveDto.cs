using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs.Planning;

public class CreateObjectiveDto
{
    public Sport Sport { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid ObjectiveCategoryId { get; set; }
    public Guid? ObjectiveSubcategoryId { get; set; }
    public List<ObjectiveTechniqueDto> Techniques { get; set; } = new();
}