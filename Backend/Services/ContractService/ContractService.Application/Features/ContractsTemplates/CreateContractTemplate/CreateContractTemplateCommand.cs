using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.CreateContractTemplate;

public record CreateContractTemplateCommand(
    string Name,
    string Content,
    string DocumentType,
    int Version = 1,
    DateTime? ValidFrom = null) : IRequest;
