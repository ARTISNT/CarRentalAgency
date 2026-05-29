using MediatR;

namespace ContractService.Application.Features.Contracts.GetContract;

public record GetContractQuery(Guid Id) : IRequest<ContractResponse>;