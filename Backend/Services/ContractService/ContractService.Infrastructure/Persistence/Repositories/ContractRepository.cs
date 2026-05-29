using ContractService.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ContractService.Infrastructure.Persistence.Repositories;

public class ContractRepository(ContractServiceContext dbContext) : IContractRepository
{
    public async Task<IReadOnlyCollection<Contract>> GetContractsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Contracts.ToListAsync(cancellationToken);
    }

    public async Task<Contract?> GetContractAsync(Guid contractId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Contracts.FirstOrDefaultAsync(r => r.Id == contractId, cancellationToken);
    }

    public async Task AddContractAsync(Contract contract, CancellationToken cancellationToken = default)
    {
        await dbContext.Contracts.AddAsync(contract, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateContractAsync(Contract contract, CancellationToken cancellationToken = default)
    {
        dbContext.Contracts.Update(contract);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}