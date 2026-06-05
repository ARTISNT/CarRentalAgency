using AutoMapper;
using ContractService.Application.Abstractions.Security;
using ContractService.Application.Authorization;
using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.Contracts.GetContracts;

public class GetContractsQueryHandler(
    IContractRepository contractRepository,
    IMapper mapper,
    IClientContext clientContext,
    IContractAuthorizationPolicy authorizationPolicy)
    : IRequestHandler<GetContractsQuery, IReadOnlyCollection<ContractListResponse>>
{
    public async Task<IReadOnlyCollection<ContractListResponse>> Handle(GetContractsQuery request, CancellationToken cancellationToken)
    {
        if(!authorizationPolicy.CanViewClientContracts())
            request.ContractSpecification.ClientId = clientContext.ClientId;
            
        var contracts = await contractRepository.GetContractsAsync(request.ContractSpecification, cancellationToken);
        if (!contracts!.Any())
            return Array.Empty<ContractListResponse>();

        return mapper.Map<IReadOnlyCollection<ContractListResponse>>(contracts);
    }
}