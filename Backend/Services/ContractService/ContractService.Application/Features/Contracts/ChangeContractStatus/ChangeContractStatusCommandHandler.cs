using ContractService.Application.Authorization;
using ContractService.Application.Exceptions.Contracts;
using ContractService.Domain.Common;
using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.Contracts.ChangeContractStatus;

public class ChangeContractStatusCommandHandler(
    IContractRepository contractRepository,
    IContractAuthorizationService contractAuthorizationService)
    : IRequestHandler<ChangeContractStatusCommand>
{
    public async Task Handle(ChangeContractStatusCommand request, CancellationToken cancellationToken)
    {
        contractAuthorizationService.EnsureCanChangeContractStatus();

        var contract = await contractRepository.GetContractAsync(request.ContractId, cancellationToken)
                       ?? throw new ContractNotFoundException("Contract not found");

        var newStatus = Enumeration.FromName<ContractStatus>(request.NewStatus);

        if (newStatus == ContractStatus.Active)
        {
            contract.Sign();
        }
        else if (newStatus == ContractStatus.Cancelled)
        {
            contract.Cancel("Status changed by administrator");
        }
        else
        {
            throw new InvalidOperationException($"Cannot change contract status to {request.NewStatus}");
        }

        await contractRepository.UpdateContractAsync(contract, cancellationToken);
    }
}