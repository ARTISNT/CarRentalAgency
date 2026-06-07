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

        var base64Data = request.SignatureBase64;
        if (base64Data.Contains(","))
            base64Data = base64Data.Split(',')[1];
        var signatureImage = Convert.FromBase64String(base64Data);

        documentService.SignContract(clientContext.ClientId, contract, signatureImage);
        contract.Sign();
        
        await contractRepository.UpdateContractAsync(contract, cancellationToken);

        var integrationEvent = new ContractSignedIntegrationEvent(
            contract.Id,
            contract.ClientId,
            contract.RentalId,
            DateTime.UtcNow);
        await publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}