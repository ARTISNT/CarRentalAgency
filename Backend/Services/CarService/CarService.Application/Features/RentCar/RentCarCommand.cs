using MediatR;

namespace CarService.Application.Features.RentCar;

public record RentCarCommand(Guid CarId) : IRequest;
