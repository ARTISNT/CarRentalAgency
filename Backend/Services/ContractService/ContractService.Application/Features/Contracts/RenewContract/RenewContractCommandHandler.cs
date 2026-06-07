using AutoMapper;
using ContractService.Application.Exceptions.Contracts;
using ContractService.Application.Options;
using ContractService.Application.Services;
using ContractService.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.Options;

namespace ContractService.Application.Features.Contracts.RenewContract;

public class RenewContractCommandHandler(
    IOptions<DocumentTemplateOptions> options,
    ContractDocumentService documentService,
    IMapper mapper,
    IContractRepository contractRepository,
    IContractTemplateRepository contractTemplateRepository)
    : IRequestHandler<RenewContractCommand>
{
    private readonly Guid _contractTemplateIdBasic = options.Value.ContractTemplateIdBasic;

    public async Task Handle(RenewContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetContractByRentalIdAsync(request.RentalId, cancellationToken)
            ?? throw new ContractNotFoundException("Contract not found");
        
        var contractTemplate = await contractTemplateRepository.GetContractTemplatesAsync(_contractTemplateIdBasic, cancellationToken)
            ?? throw new ContractNotFoundException("Contract template not found");

        var contractTemplateSnapshot = mapper.Map<ContractTemplateSnapshot>(contractTemplate);
        
        contract!.RenewContract(request.NewEndDate, request.AdditionalPrice, contractTemplateSnapshot);
        
        await contractRepository.UpdateContractAsync(contract, cancellationToken);
        await documentService.GenerateAddition(contract.ClientId, contractTemplateSnapshot.Content, contract);
        documentService.SignAddition(contract.ClientId, contract);
    }
}