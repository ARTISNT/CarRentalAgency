using ContractService.Application.Exceptions.Contracts;
using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.Contracts.CancelContract;

public class CancelContractCommandHandler(IContractRepository contractRepository) : IRequestHandler<CancelContract.CancelContractCommand>
{
    public async Task Handle(CancelContract.CancelContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetContractAsync(request.ContractId, cancellationToken)
                       ?? throw new ContractNotFoundException("Contract not found");
        
        contract.Cancel(request.Reason);
        await contractRepository.UpdateContractAsync(contract, cancellationToken);
    }
}