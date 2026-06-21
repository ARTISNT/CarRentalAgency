using MediatR;
using Contracts.Common;

namespace CarService.Application.Features.GetCarForContract;

public record GetCarForContractQuery(Guid Id) : IRequest<CarForContractResponse>, IAuthorizedRequest;