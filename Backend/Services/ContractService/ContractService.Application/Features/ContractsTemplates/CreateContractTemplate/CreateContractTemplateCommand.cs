using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.CreateContractTemplate;

public record CreateContractTemplateCommand(
    string Name,
    string Content,
    int Version,
    DateTime ValidFrom) : IRequest;