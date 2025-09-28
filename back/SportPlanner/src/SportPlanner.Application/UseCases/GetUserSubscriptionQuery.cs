using MediatR;
using SportPlanner.Application.DTOs;

namespace SportPlanner.Application.UseCases;

public record GetUserSubscriptionQuery : IRequest<SubscriptionResponse?>;
