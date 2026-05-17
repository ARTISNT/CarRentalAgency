using MediatR;

namespace CarService.Application.Features.GetCars;

public record GetCarsQuery() : IRequest<IReadOnlyCollection<CarListResponse>>;