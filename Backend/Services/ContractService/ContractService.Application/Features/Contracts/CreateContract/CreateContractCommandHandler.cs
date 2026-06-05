using AutoMapper;
using Contracts.ContractEvents;
using ContractService.Application.Abstractions.External;
using ContractService.Application.Abstractions.Security;
using ContractService.Application.Authorization;
using ContractService.Application.Exceptions.Contracts;
using ContractService.Application.Services;
using ContractService.Domain.Contracts;
using MassTransit;
using MediatR;

namespace ContractService.Application.Features.Contracts.CreateContract;

public class CreateContractCommandHandler(
    IContractRepository contractRepository,
    IContractTemplateRepository contractTemplateRepository,
    ICarExternalService carExternalService,
    IRentalExternalService rentalExternalService,
    IClientExternalService clientExternalService,
    IClientContext clientContext,
    IContractAuthorizationService contractAuthorizationService,
    IMapper mapper,
    ContractDocumentService contractDocumentService,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<CreateContractCommand>
{
    public async Task Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var contractTemplate = await contractTemplateRepository.GetContractTemplatesAsync(request.ContractTemplateId, cancellationToken) 
                               ?? throw new ContractNotFoundException("Contract template not found");
        
        var clientId = request.ClientId ?? clientContext.ClientId;
        contractAuthorizationService.EnsureCanCreateContracts(clientId);

        var carTask = carExternalService.GetCarForContractAsync(request.CarId, cancellationToken);
        var rentalTask = rentalExternalService.GetRentalForContractAsync(request.RentalId, cancellationToken);
        var clientTask = clientExternalService.GetClientForRentAsync(clientId, cancellationToken);

        await Task.WhenAll(carTask, rentalTask, clientTask);

        var clientResponse = await clientTask;
        var carResponse = await carTask;
        var rentalResponse = await rentalTask;

        var contract = new Contract(
            request.ContractTemplateId,
            request.CarId,
            clientId,
            rentalResponse.RentalId,
            mapper.Map<ClientSnapshot>(clientResponse),
            mapper.Map<ContractAutoSnapshot>(carResponse),
            mapper.Map<ContractTemplateSnapshot>(contractTemplate),
            mapper.Map<RentalSnapshot>(rentalResponse));

        await contractRepository.AddContractAsync(contract, cancellationToken);
        await contractDocumentService.GenerateContract(
            clientId,
            contractTemplate.Content,
            contract);

        var integrationEvent = new ContractCreatedIntegrationEvent(
            contract.Id,
            clientId,
            contract.RentalId,
            contract.CreatedAt);
        await publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}