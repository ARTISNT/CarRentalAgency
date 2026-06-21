using Contracts.Common;
using ContractService.Application.Features.Contracts.GetDetailedContract;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.GetContractTemplate;

public record GetContractTemplateQuery(Guid Id) : IRequest<ContractTemplateResponse>, IAuthorizedRequest;