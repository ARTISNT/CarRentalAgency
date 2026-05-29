using AutoMapper;
using ContractService.Application.Abstractions.External;
using ContractService.Application.Abstractions.Security;
using ContractService.Application.Services;
using ContractService.Domain.Contracts;
using ContractService.Domain.Exceptions.Contracts;
using MediatR;

namespace ContractService.Application.Features.Contracts.CreateContract;

public class CreateContractCommandHandler(
    IContractRepository contractRepository,
    IContractTemplateRepository contractTemplateRepository,
    ICarExternalService carExternalService,
    IRentalExternalService rentalExternalService,
    IClientExternalService clientExternalService,
    IUserContext userContext,
    IMapper mapper,
    ContractDocumentService contractDocumentService)
    : IRequestHandler<CreateContractCommand>
{
    public async Task Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var contractTemplate = await contractTemplateRepository.GetContractTemplatesAsync(request.ContractTemplateId, cancellationToken) 
                               ?? throw new ContractNotFoundException("Contract template not found");
        
        var carExternalResponse = await carExternalService.GetCarForContractAsync(request.CarId, cancellationToken);
        var rentalExternalResponse = await rentalExternalService.GetRentalForContractAsync(request.RentalId, cancellationToken);
        var clientExternalResponse = await clientExternalService.GetClientForRentAsync(request.ClientId, cancellationToken);

        var contractSnapshot = mapper.Map<ContractTemplateSnapshot>(contractTemplate);
        var client = mapper.Map<ClientSnapshot>(clientExternalResponse);
        var auto = mapper.Map<ContractAutoSnapshot>(carExternalResponse);
        var rental = mapper.Map<RentalSnapshot>(rentalExternalResponse);
        
        var contract = new Contract(request.ContractTemplateId, client,  auto, contractSnapshot, rental);
        
        await contractRepository.AddContractAsync(contract, cancellationToken);
        await contractDocumentService.GenerateContract(userContext.UserId, contractTemplate.Content, contract);
    }
}