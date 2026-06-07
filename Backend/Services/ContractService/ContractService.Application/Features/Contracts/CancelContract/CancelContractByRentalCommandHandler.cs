using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.Contracts.CancelContract;

public class CancelContractByRentalCommandHandler(IContractRepository contractRepository)
    : IRequestHandler<CancelContractByRentalCommand>
{
    public async Task Handle(CancelContractByRentalCommand request, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetContractByRentalIdAsync(request.RentalId, cancellationToken);

        if (contract is null || contract.Status != ContractStatus.AwaitingSignature)
            return;

        contract.Cancel(request.Reason ?? "Rental cancelled");
        await contractRepository.UpdateContractAsync(contract, cancellationToken);
    }
}
