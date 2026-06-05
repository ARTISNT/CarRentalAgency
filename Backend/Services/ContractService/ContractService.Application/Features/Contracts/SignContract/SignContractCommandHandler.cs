using Contracts.ContractEvents;
using ContractService.Application.Abstractions.Security;
using ContractService.Application.Exceptions.Contracts;
using ContractService.Application.Services;
using ContractService.Domain.Contracts;
using MassTransit;
using MediatR;

namespace ContractService.Application.Features.Contracts.SignContract;

public class SignContractCommandHandler(
    IContractRepository contractRepository,
    ContractDocumentService documentService,
    IClientContext clientContext,
    IPublishEndpoint publishEndpoint) 
    : IRequestHandler<SignContractCommand>
{
    public async Task Handle(SignContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetContractAsync(request.Id, cancellationToken)
            ?? throw new ContractNotFoundException("Contract not found");

        documentService.SignContract(clientContext.ClientId, contract);
        contract.Sign();
        
        await contractRepository.UpdateContractAsync(contract, cancellationToken);

        var integrationEvent = new ContractSignedIntegrationEvent(
            contract.Id,
            contract.ClientId,
            DateTime.UtcNow);
        await publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}