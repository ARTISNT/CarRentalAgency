using AutoMapper;
using Contracts.ContractEvents;
using ContractService.Application.Exceptions.Contracts;
using ContractService.Application.Options;
using ContractService.Application.Services;
using ContractService.Domain.Contracts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;

namespace ContractService.Application.Features.Contracts.EndContract;

public class EndContractByRentalCommandHandler(
    IContractRepository contractRepository,
    IContractTemplateRepository contractTemplateRepository,
    IOptions<DocumentTemplateOptions> options,
    IMapper mapper,
    ContractDocumentService documentService,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<EndContractByRentalCommand>
{
    public async Task Handle(EndContractByRentalCommand request, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetContractByRentalIdAsync(request.RentalId, cancellationToken)
            ?? throw new ContractNotFoundException("Contract not found for rental");

        var template = await contractTemplateRepository.GetContractTemplatesAsync(
                options.Value.ReturnActTemplateId, cancellationToken)
            ?? throw new ContractNotFoundException("ReturnAct template not found");

        var templateSnapshot = mapper.Map<ContractTemplateSnapshot>(template);

        var returnAct = new ContractReturnAct(
            request.Mileage,
            request.FuelLevel,
            request.PenaltyAmount,
            request.DamageDescription,
            templateSnapshot);

        contract.EndContract(returnAct);

        await contractRepository.UpdateContractAsync(contract, cancellationToken);
        await documentService.GenerateReturnAct(contract.ClientId, template.Content, contract);
        documentService.SignReturnAct(contract.ClientId, contract);

        await publishEndpoint.Publish(new ContractEndedIntegrationEvent(
            contract.Id,
            contract.RentalId,
            contract.ContractAutoId,
            contract.ClientId,
            request.Mileage,
            request.FuelLevel,
            request.PenaltyAmount,
            request.DamageDescription,
            DateTime.UtcNow), cancellationToken);
    }
}
