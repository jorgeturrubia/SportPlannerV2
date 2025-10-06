using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs.Planning;

public class CreateObjectiveCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public Sport Sport { get; set; }
}
