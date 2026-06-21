using Contracts.Common;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.DeactivateContractTemplate;

public record DeactivateContractTemplateCommand(Guid Id) : IRequest, IAuthorizedRequest;