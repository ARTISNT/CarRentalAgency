using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.GetAvailableVariables;

public record GetAvailableVariablesQuery(string DocumentType) : IRequest<List<TemplateVariableResponse>>;
