namespace ContractService.Domain.Contracts;

public interface IContractTemplateRepository
{
    public Task<IReadOnlyCollection<ContractTemplate>> GetContractsTemplatesAsync(
        CancellationToken cancellationToken = default);
    
    public Task<ContractTemplate?> GetContractTemplatesAsync(Guid contractTemplateId,
        CancellationToken cancellationToken = default);

    public Task AddContractTemplateAsync(ContractTemplate contractTemplate,
        CancellationToken cancellationToken = default);

    public Task UpdateContractTemplateAsync(ContractTemplate contractTemplate,
        CancellationToken cancellationToken = default);
}