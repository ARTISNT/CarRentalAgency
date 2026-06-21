using Contracts.Common;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.UpdateContractTemplateContent;

public record UpdateContractTemplateContentCommand(Guid Id, string Content) : IRequest, IAuthorizedRequest;