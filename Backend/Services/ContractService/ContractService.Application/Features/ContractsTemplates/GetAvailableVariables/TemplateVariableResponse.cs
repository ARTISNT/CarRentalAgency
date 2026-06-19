namespace ContractService.Application.Features.ContractsTemplates.GetAvailableVariables;

public record TemplateVariableResponse(
    string Key,
    string Description,
    string Group,
    string Example);
