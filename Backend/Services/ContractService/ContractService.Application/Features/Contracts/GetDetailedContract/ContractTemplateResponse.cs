namespace ContractService.Application.Features.Contracts.GetDetailedContract;

public class ContractTemplateResponse
{
    public int Version { get; init; }
    public string Name { get; init; }
    public string Content { get; init; }
    public DateTime ValidFrom { get; init; }
    public bool IsActive { get; init; } 
}