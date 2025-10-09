using MediatR;
using SportPlanner.Application.DTOs.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public class GetExerciseCategoriesQueryHandler : IRequestHandler<GetExerciseCategoriesQuery, List<ExerciseCategoryDto>>
{
    // TODO: Implement with repository interface when ready
    public async Task<List<ExerciseCategoryDto>> Handle(GetExerciseCategoriesQuery request, CancellationToken cancellationToken)
    {
        // Placeholder implementation - return empty list for now
        await Task.CompletedTask;
        return new List<ExerciseCategoryDto>();
    }
}
