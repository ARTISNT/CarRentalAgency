using MediatR;

namespace CarService.Application.Features.GetDetailedCars;

public record GetDetailedCarQuery(Guid CarId) : IRequest<CarDetailsResponse>;