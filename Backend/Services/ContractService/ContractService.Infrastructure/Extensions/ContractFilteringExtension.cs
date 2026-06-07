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

        if (!string.IsNullOrWhiteSpace(specification.Status))
        {
            query = query.Where(c => c.Status.Name == specification.Status);
        }

        if (specification.DateFrom.HasValue)
        {
            query = query.Where(c => c.CreatedAt >= specification.DateFrom.Value);
        }

        if (specification.DateTo.HasValue)
        {
            query = query.Where(c => c.CreatedAt <= specification.DateTo.Value);
        }

        query = query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((specification.Page - 1) * specification.PageSize)
            .Take(specification.PageSize);

        return query;
    }
}