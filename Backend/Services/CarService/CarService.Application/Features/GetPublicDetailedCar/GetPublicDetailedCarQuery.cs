using CarService.Application.Features.GetDetailedCars;
using Contracts.Common;
using MediatR;

namespace CarService.Application.Features.GetPublicDetailedCar;

public record GetPublicDetailedCarQuery(Guid CarId) : IRequest<PublicCarDetailsResponse>, IAuthorizedRequest;
