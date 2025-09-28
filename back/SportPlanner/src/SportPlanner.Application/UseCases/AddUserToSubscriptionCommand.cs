using MediatR;
using SportPlanner.Application.DTOs;

namespace SportPlanner.Application.UseCases;

public record AddUserToSubscriptionCommand(
    Guid SubscriptionId,
    AddUserToSubscriptionRequest Request) : IRequest<Unit>;
