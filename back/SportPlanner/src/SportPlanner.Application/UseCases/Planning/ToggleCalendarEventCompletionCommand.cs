using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

public record ToggleCalendarEventCompletionCommand(Guid Id) : IRequest<Unit>;
