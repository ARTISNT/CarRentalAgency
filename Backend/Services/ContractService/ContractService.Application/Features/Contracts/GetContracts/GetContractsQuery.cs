using MediatR;

namespace ContractService.Application.Features.Contracts.GetContracts;

public record GetContractsQuery() : IRequest<IReadOnlyCollection<ContractListResponse>>;