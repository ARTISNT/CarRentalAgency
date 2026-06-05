using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.CreateContractTemplate;

public record CreateContractTemplateCommand(
    string Name,
    string Content,
    int Version,
    DocumentType DocumentType,
    DateTime ValidFrom) : IRequest;