using ContractService.Domain.Contracts;

namespace ContractService.Infrastructure.Extensions;

public static class ContractFilteringExtension
{
    public static IQueryable<Contract> ApplyFiltering(
        this IQueryable<Contract> query,
        ContractSpecification specification)
    {
        if (specification.ClientId.HasValue)
        {
            query = query.Where(c => c.ClientId == specification.ClientId);
        }

        return query;
    }
}