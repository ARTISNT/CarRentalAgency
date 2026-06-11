using AutoMapper;
using ContractService.Application.Abstractions.Security;
using ContractService.Application.Exceptions;
using ContractService.Application.Exceptions.Contracts;
using ContractService.Application.Features.Contracts.GetContracts;
using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.Contracts.GetContract;

public class GetContractQueryHandler(
    IContractRepository contractRepository,
    IMapper mapper,
    IClientContext clientContext) 
    : IRequestHandler<GetContractQuery, ContractListResponse>
{
    public async Task<ContractListResponse> Handle(GetContractQuery request, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetContractAsync(request.Id, cancellationToken)
                       ?? throw new ContractNotFoundException("Contract not found");

        if (contract.ClientId != clientContext.ClientId &&
            !clientContext.Permissions.Contains("ViewAllContracts"))
            throw new ForbiddenException("No permission");

        return mapper.Map<ContractListResponse>(contract);
    }
}