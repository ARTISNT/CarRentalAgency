using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.GetContractTemplates;

public record GetContractTemplatesQuery() : IRequest<IReadOnlyCollection<ContractTemplateListResponse>>;