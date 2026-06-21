using CarService.Application.Features.GetCars;
using Contracts.Common;
using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.GetAvailableCars;

public record GetAvailableCarsQuery(CarSpecification Specification) : IRequest<IReadOnlyCollection<CarListResponse>>, IAuthorizedRequest;
