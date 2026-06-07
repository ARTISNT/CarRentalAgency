using CarService.Application.Features.GetDetailedCars;
using MediatR;

namespace CarService.Application.Features.GetPublicDetailedCar;

public record GetPublicDetailedCarQuery(Guid CarId) : IRequest<PublicCarDetailsResponse>;
