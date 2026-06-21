using CarService.Domain.Cars;
using Contracts.Common;
using MediatR;

namespace CarService.Application.Features.GetCars;

public record GetCarsQuery(CarSpecification CarSpecification) : IRequest<IReadOnlyCollection<CarListResponse>>, IAuthorizedRequest;