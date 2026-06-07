using MediatR;

namespace CarService.Application.Features.BreakCar;

public record BreakCarCommand(Guid CarId) : IRequest;
