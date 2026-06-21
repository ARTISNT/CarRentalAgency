using MediatR;
using Contracts.Common;

namespace CarService.Application.Features.GetCarForRent;

public record GetCarForRentQuery(Guid Id) : IRequest<CarForRentResponse>, IAuthorizedRequest; 