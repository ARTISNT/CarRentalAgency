using AutoMapper;
using ContractService.Application.Abstractions.Services;
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
    IContractTemplateRepository contractTemplateRepository,
    ITemplateRenderer templateRenderer,
    ContractTemplateVariablesBuilder variablesBuilder)
    : IRequestHandler<RenewContractCommand>
{
    private readonly Guid _additionTemplateId = options.Value.AdditionTemplateId;

    public async Task Handle(RenewContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetContractByRentalIdAsync(request.RentalId, cancellationToken)
            ?? throw new ContractNotFoundException("Contract not found");

        var contractTemplate = await contractTemplateRepository.GetContractTemplatesAsync(_additionTemplateId, cancellationToken)
            ?? throw new ContractNotFoundException("Contract template not found");

        var contractTemplateSnapshot = mapper.Map<ContractTemplateSnapshot>(contractTemplate);

        contract!.RenewContract(request.NewEndDate, request.AdditionalPrice, contractTemplateSnapshot);

        await contractRepository.UpdateContractAsync(contract, cancellationToken);

        var lastAddition = contract.ContractAdditions.Last();
        var variables = variablesBuilder.ForAddition(contract, lastAddition);
        var renderedContent = templateRenderer.Render(contractTemplateSnapshot.Content, variables);

        await documentService.GenerateAddition(contract.ClientId, renderedContent, contract);
        documentService.SignAddition(contract.ClientId, contract);
    }
}