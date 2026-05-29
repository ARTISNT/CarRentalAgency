namespace ContractService.Domain.Contracts;

public interface IContractRepository
{
    public Task<IReadOnlyCollection<Contract>> GetContractsAsync(CancellationToken cancellationToken = default);
    public Task<Contract?> GetContractAsync(Guid contractId, CancellationToken cancellationToken = default);
    public Task AddContractAsync(Contract contract, CancellationToken cancellationToken = default);
    public Task UpdateContractAsync(Contract contract, CancellationToken cancellationToken = default);
}