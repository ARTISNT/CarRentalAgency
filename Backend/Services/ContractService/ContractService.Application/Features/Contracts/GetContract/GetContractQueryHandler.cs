using AutoMapper;
using ContractService.Domain.Contracts;
using ContractService.Domain.Exceptions.Contracts;
using MediatR;

namespace ContractService.Application.Features.Contracts.GetContract;

public class GetContractQueryHandler(IContractRepository contractRepository, IMapper mapper) : IRequestHandler<GetContractQuery, ContractResponse>
{
    public async Task<ContractResponse> Handle(GetContractQuery request, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetContractAsync(request.Id, cancellationToken)
                       ?? throw new ContractNotFoundException("Contract not found");

        return mapper.Map<ContractResponse>(contract);
    }
}