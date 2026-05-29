using ContractService.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ContractService.Infrastructure.Persistence.Repositories;

public class ContractTemplateTemplateRepository(ContractServiceContext dbContext) : IContractTemplateRepository
{
    public async Task<IReadOnlyCollection<ContractTemplate>> GetContractsTemplatesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.ContractTemplates.ToListAsync(cancellationToken);
    }

    public async Task<ContractTemplate?> GetContractTemplatesAsync(Guid contractTemplateId, CancellationToken cancellationToken = default)
    {
        return await dbContext.ContractTemplates.FirstOrDefaultAsync(r => r.Id == contractTemplateId,cancellationToken );
    }

    public async Task AddContractTemplateAsync(ContractTemplate contractTemplate, CancellationToken cancellationToken = default)
    {
        await dbContext.ContractTemplates.AddAsync(contractTemplate, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateContractTemplateAsync(ContractTemplate contractTemplate, CancellationToken cancellationToken = default)
    {
        dbContext.ContractTemplates.Update(contractTemplate);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}