using RentalService.Domain.Rentals;

namespace RentalService.Infrastructure.Extensions;

public static class RentalFilteringExtension
{
    public static IQueryable<Rental> ApplyFiltering(
        this IQueryable<Rental> query,
        RentalSpecification specification)
    {
        if (specification.CarRenterId.HasValue)
        {
            query = query.Where(r => r.CarRenterId == specification.CarRenterId);
        }

        if (!string.IsNullOrWhiteSpace(specification.Status))
        {
            query = query.Where(r => r.ActivityStatus.Name == specification.Status);
        }

        if (specification.DateFrom.HasValue)
        {
            query = query.Where(r => r.StartDate >= specification.DateFrom.Value);
        }

        if (specification.DateTo.HasValue)
        {
            query = query.Where(r => r.StartDate <= specification.DateTo.Value);
        }

        query = query
            .OrderByDescending(r => r.StartDate)
            .Skip((specification.Page - 1) * specification.PageSize)
            .Take(specification.PageSize);

        return query;
    }
}
