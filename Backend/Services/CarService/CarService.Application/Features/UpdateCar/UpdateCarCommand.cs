using MediatR;

namespace CarService.Application.Features.UpdateCar;

public record UpdateCarCommand(Guid CarId, UpdateCarDto UpdateCarDto) : IRequest;