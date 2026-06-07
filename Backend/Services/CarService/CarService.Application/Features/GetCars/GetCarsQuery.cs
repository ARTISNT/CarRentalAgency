using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.GetCars;

public record GetCarsQuery(CarSpecification CarSpecification) : IRequest<IReadOnlyCollection<CarListResponse>>;