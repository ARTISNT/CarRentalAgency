using CarService.Application.Features.GetCars;
using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.GetAvailableCars;

public record GetAvailableCarsQuery(CarSpecification Specification) : IRequest<IReadOnlyCollection<CarListResponse>>;
