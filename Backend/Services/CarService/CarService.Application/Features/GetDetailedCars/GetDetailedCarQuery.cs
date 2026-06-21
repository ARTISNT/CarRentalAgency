using MediatR;
using Contracts.Common;

namespace CarService.Application.Features.GetDetailedCars;

public record GetDetailedCarQuery(Guid CarId) : IRequest<CarDetailsResponse>, IAuthorizedRequest;