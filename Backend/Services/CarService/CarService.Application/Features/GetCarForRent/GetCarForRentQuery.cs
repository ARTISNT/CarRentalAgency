using MediatR;

namespace CarService.Application.Features.GetCarForRent;

public record GetCarForRentQuery(Guid Id) : IRequest<CarForRentResponse>; 