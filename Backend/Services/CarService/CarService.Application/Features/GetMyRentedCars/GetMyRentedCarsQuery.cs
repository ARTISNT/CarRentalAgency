using CarService.Application.Features.GetCars;
using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.GetMyRentedCars;

public record GetMyRentedCarsQuery(CarSpecification Specification) : IRequest<IReadOnlyCollection<CarListResponse>>;
