using MediatR;
using SportPlanner.Application.DTOs.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public class GetExerciseTypesQueryHandler : IRequestHandler<GetExerciseTypesQuery, List<ExerciseTypeDto>>
{
    // TODO: Implement with repository interface when ready
    public async Task<List<ExerciseTypeDto>> Handle(GetExerciseTypesQuery request, CancellationToken cancellationToken)
    {
        // Placeholder implementation - return empty list for now
        await Task.CompletedTask;
        return new List<ExerciseTypeDto>();
    }
}
