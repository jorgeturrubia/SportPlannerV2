using MediatR;
using SportPlanner.Application.DTOs.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public record UpdateTrainingPlanCommand(UpdateTrainingPlanDto TrainingPlan) : IRequest<Unit>;