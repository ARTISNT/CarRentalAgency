using MediatR;

namespace CarService.Application.Features.AddCar;

public record AddCarCommand(CreateCarDto CreateCarDto) : IRequest;