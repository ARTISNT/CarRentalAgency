using CarService.Application.Features.GetCars;
using Contracts.Common;
using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.GetMyRentedCars;

public record GetMyRentedCarsQuery(CarSpecification Specification) : IRequest<IReadOnlyCollection<CarListResponse>>, IAuthorizedRequest;
