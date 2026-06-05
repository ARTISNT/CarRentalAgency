using AutoMapper;
using ContractService.Application.Authorization;
using ContractService.Application.Exceptions.Contracts;
using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.Contracts.GetContract;

public class GetContractQueryHandler(
    IContractRepository contractRepository,
    IMapper mapper,
    IContractAuthorizationService contractAuthorizationService) 
    : IRequestHandler<GetContractQuery, ContractResponse>
{
    public async Task<ContractResponse> Handle(GetContractQuery request, CancellationToken cancellationToken)
    {
        contractAuthorizationService.EnsureCanViewContracts();
        var contract = await contractRepository.GetContractAsync(request.Id, cancellationToken)
                       ?? throw new ContractNotFoundException("Contract not found");

        return mapper.Map<ContractResponse>(contract);
    }
}