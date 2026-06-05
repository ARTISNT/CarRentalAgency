using AutoMapper;
using ContractService.Application.Exceptions.Contracts;
using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.Contracts.GetDetailedContract;

public class GetDetailedContractQueryHandler(
    IContractRepository contractRepository,
    IMapper mapper)
    : IRequestHandler<GetDetailedContractQuery, DetailedContractResponse>
{
    public async Task<DetailedContractResponse> Handle(GetDetailedContractQuery request, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetContractAsync(request.Id, cancellationToken)
                       ?? throw new ContractNotFoundException("Contract not found");

        return mapper.Map<DetailedContractResponse>(contract);
    }
}