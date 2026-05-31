namespace ContractService.Application.Features.ContractsTemplates.GetContractTemplates;

public class ContractTemplateListResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public int Version { get; init; }
    public DateTime ValidFrom { get; init; }
    public bool IsActive { get; init; }
}