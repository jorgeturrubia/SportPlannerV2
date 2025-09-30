using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs.Planning;

public class ObjectiveCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Sport Sport { get; set; }
}