using MediatR;

namespace CarService.Application.Features.GetCarForContract;

public record GetCarForContractQuery(Guid Id) : IRequest<CarForContractResponse>;