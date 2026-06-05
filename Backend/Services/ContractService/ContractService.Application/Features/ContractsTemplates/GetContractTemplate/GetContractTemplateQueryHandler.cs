using ContractService.Application.Exceptions.Contracts;
using ContractService.Application.Features.Contracts.GetDetailedContract;
using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.GetContractTemplate;

public class GetContractTemplateQueryHandler(
    IContractTemplateRepository contractTemplateRepository)
    : IRequestHandler<GetContractTemplateQuery, ContractTemplateResponse>
{
    public async Task<ContractTemplateResponse> Handle(
        GetContractTemplateQuery request, CancellationToken cancellationToken)
    {
        var template = await contractTemplateRepository.GetContractTemplatesAsync(request.Id, cancellationToken)
                       ?? throw new ContractTemplateNotFoundException("Contract template not found");

        return new ContractTemplateResponse
        {
            Version = template.Version,
            Name = template.Name,
            Content = template.Content,
            ValidFrom = template.ValidFrom,
            IsActive = template.IsActive
        };
    }
}