using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.Contracts.GetContracts;

public record GetContractsQuery(ContractSpecification ContractSpecification) : IRequest<IReadOnlyCollection<ContractListResponse>>;