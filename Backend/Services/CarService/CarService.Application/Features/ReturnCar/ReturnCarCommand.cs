using MediatR;

namespace CarService.Application.Features.ReturnCar;

public record ReturnCarCommand(Guid CarId) : IRequest;
