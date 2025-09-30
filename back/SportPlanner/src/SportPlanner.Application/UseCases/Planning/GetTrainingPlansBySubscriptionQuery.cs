using MediatR;
using SportPlanner.Application.DTOs.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public record GetTrainingPlansBySubscriptionQuery(bool IncludeInactive = false) : IRequest<List<TrainingPlanDto>>;