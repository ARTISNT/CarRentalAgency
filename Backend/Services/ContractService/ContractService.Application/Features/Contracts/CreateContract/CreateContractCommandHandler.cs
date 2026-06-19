using AutoMapper;
using Contracts.ContractEvents;
using ContractService.Application.Abstractions.External;
using ContractService.Application.Abstractions.Services;
using ContractService.Application.Exceptions.Contracts;
using ContractService.Application.Options;
using ContractService.Application.Services;
using ContractService.Domain.Contracts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;

namespace ContractService.Application.Features.Contracts.CreateContract;

public class CreateContractCommandHandler(
    IContractRepository contractRepository,
    IContractTemplateRepository contractTemplateRepository,
    ICarExternalService carExternalService,
    IRentalExternalService rentalExternalService,
    IClientExternalService clientExternalService,
    IOptions<DocumentTemplateOptions> options,
    IMapper mapper,
    ContractDocumentService contractDocumentService,
    IPublishEndpoint publishEndpoint,
    ITemplateRenderer templateRenderer,
    ContractTemplateVariablesBuilder variablesBuilder)
    : IRequestHandler<CreateContractCommand>
{
    public async Task Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var contractTemplate = await contractTemplateRepository.GetContractTemplatesAsync(options.Value.ContractTemplateId, cancellationToken)
                               ?? throw new ContractNotFoundException("Contract template not found");

        var carTask = carExternalService.GetCarForContractAsync(request.CarId, cancellationToken);
        var rentalTask = rentalExternalService.GetRentalForContractAsync(request.RentalId, cancellationToken);
        var clientTask = clientExternalService.GetClientForRentAsync(request.ClientId, cancellationToken);

        await Task.WhenAll(carTask, rentalTask, clientTask);

        var clientResponse = await clientTask;
        var carResponse = await carTask;
        var rentalResponse = await rentalTask;

        var contract = new Contract(
            options.Value.ContractTemplateId,
            request.CarId,
            request.ClientId,
            rentalResponse.RentalId,
            mapper.Map<ClientSnapshot>(clientResponse),
            mapper.Map<ContractAutoSnapshot>(carResponse),
            mapper.Map<ContractTemplateSnapshot>(contractTemplate),
            mapper.Map<RentalSnapshot>(rentalResponse));

        await contractRepository.AddContractAsync(contract, cancellationToken);

        var variables = variablesBuilder.ForContract(contract);
        var renderedContent = templateRenderer.Render(contractTemplate.Content, variables);

        await contractDocumentService.GenerateContract(
            request.ClientId,
            renderedContent,
            contract);

        var integrationEvent = new ContractCreatedIntegrationEvent(
            contract.Id,
            request.ClientId,
            contract.RentalId,
            contract.CreatedAt);
        await publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}