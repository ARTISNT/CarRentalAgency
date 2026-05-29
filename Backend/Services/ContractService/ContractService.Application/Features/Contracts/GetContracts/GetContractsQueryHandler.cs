using AutoMapper;
using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.Contracts.GetContracts;

public class GetContractsQueryHandler(IContractRepository contractRepository, IMapper mapper) : IRequestHandler<GetContractsQuery, IReadOnlyCollection<ContractListResponse>>
{
    public async Task<IReadOnlyCollection<ContractListResponse>> Handle(GetContractsQuery request, CancellationToken cancellationToken)
    {
        var contracts = await contractRepository.GetContractsAsync(cancellationToken);
        if (!contracts!.Any())
            return Array.Empty<ContractListResponse>();

        return mapper.Map<IReadOnlyCollection<ContractListResponse>>(contracts);
    }
}