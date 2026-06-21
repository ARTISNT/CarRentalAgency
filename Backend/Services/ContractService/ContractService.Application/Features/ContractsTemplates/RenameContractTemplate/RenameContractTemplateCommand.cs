using Contracts.Common;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.RenameContractTemplate;

public record RenameContractTemplateCommand(Guid Id, string Name) : IRequest, IAuthorizedRequest;