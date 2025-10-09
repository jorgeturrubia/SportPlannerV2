using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

public record DeleteCalendarEventCommand(Guid Id) : IRequest<Unit>;
