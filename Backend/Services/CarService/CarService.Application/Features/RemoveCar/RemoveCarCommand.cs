using MediatR;

namespace CarService.Application.Features.RemoveCar;

public record RemoveCarCommand(Guid CarId) : IRequest;