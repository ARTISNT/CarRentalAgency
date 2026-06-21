using Contracts.Common;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.ActivateContractTemplate;

public record ActivateContractTemplateCommand(Guid Id) : IRequest, IAuthorizedRequest;