using CarService.Domain.Cars;
using CarService.Domain.Cars.Enums;
using CarService.Domain.Common;

namespace CarService.Infrastructure.Extensions;

public static class CarFilteringExtension
{
    public static IQueryable<Car> ApplyFiltering(
        this IQueryable<Car> query,
        CarSpecification specification)
    {
        if (!string.IsNullOrWhiteSpace(specification.Status))
        {
            var status = Enumeration.FromName<AvailabilityStatus>(specification.Status);
            query = query.Where(c => c.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(specification.Class))
        {
            var carClass = Enumeration.FromName<CarClass>(specification.Class);
            query = query.Where(c => c.Class == carClass);
        }

        if (specification.DateFrom.HasValue)
        {
            query = query.Where(c => c.ReleaseDate >= specification.DateFrom.Value);
        }

        if (specification.DateTo.HasValue)
        {
            query = query.Where(c => c.ReleaseDate <= specification.DateTo.Value);
        }

        if (specification.RentedBy.HasValue)
        {
            query = query.Where(c => c.CurrentRenterId == specification.RentedBy.Value);
        }

        query = query
            .OrderByDescending(c => c.ReleaseDate)
            .Skip((specification.Page - 1) * specification.PageSize)
            .Take(specification.PageSize);

        return query;
    }
}
