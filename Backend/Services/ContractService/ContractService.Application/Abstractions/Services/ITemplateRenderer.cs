namespace ContractService.Application.Abstractions.Services;

public interface ITemplateRenderer
{
    string Render(string template, IReadOnlyDictionary<string, object?> variables);
}
