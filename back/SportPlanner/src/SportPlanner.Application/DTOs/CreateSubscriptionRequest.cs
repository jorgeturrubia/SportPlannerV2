using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs;

public record CreateSubscriptionRequest(SubscriptionType Type, Sport Sport);
