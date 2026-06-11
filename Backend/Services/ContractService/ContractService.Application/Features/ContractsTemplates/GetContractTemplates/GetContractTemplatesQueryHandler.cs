using ContractService.Application.Features.ContractsTemplates.GetContractTemplates;
using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.GetContractTemplates;

public class GetContractTemplatesQueryHandler(
    IContractTemplateRepository contractTemplateRepository)
    : IRequestHandler<GetContractTemplatesQuery, IReadOnlyCollection<ContractTemplateListResponse>>
{
    public async Task<IReadOnlyCollection<ContractTemplateListResponse>> Handle(
        GetContractTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await contractTemplateRepository.GetContractsTemplatesAsync(cancellationToken);
        if (!templates.Any())
            return Array.Empty<ContractTemplateListResponse>();

        return templates.Select(t => new ContractTemplateListResponse
        {
            Id = t.Id,
            Name = t.Name,
            Version = t.Version,
            ValidFrom = t.ValidFrom,
            CreatedOn = t.CreatedOn,
            DocumentType = t.DocumentType.Name,
            IsActive = t.IsActive
        }).ToList();
    }
}