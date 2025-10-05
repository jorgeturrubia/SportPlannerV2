using MediatR;

namespace SportPlanner.Application.UseCases;

public record DeleteGenderCommand(Guid Id) : IRequest<bool>;
