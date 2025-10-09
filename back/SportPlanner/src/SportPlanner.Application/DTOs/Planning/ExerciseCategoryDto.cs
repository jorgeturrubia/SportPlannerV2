using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs.Planning;

public class ExerciseCategoryDto
{
    public Guid Id { get; set; }
    public Sport Sport { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateExerciseCategoryDto
{
    public Sport Sport { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateExerciseCategoryDto
{
    public Sport Sport { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
