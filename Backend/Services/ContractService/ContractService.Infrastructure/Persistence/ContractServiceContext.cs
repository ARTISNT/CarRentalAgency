using ContractService.Domain.Contracts;
using ContractService.Infrastructure.Persistence.EntitiesConfigurations;
using Microsoft.EntityFrameworkCore;

namespace ContractService.Infrastructure.Persistence;

public class ContractServiceContext(DbContextOptions<ContractServiceContext> options) : DbContext(options)
{
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractTemplate> ContractTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ContractConfiguration());
        modelBuilder.ApplyConfiguration(new ContractTemplateConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}