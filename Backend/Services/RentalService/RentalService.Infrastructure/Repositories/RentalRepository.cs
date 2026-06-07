using Microsoft.EntityFrameworkCore;
using RentalService.Domain.Rentals;
using RentalService.Infrastructure.Extensions;

namespace RentalService.Infrastructure.Repositories;

public class RentalRepository(RentalServiceContext dbContext) : IRentalRepository
{
    public async Task<IReadOnlyCollection<Rental>> GetRentalsAsync(RentalSpecification rentalSpecification, CancellationToken cancellationToken = default)
    {
        return await dbContext.Rentals.ApplyFiltering(rentalSpecification).ToListAsync(cancellationToken);
    }
    
    public async Task<Rental?> GetRentalAsync(Guid rentalId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Rentals.FirstOrDefaultAsync(r => r.Id == rentalId, cancellationToken);
    }

    public async Task AddRentalAsync(Rental rental, CancellationToken cancellationToken = default)
    {
        await dbContext.Rentals.AddAsync(rental, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRentalAsync(Rental rental, CancellationToken cancellationToken = default)
    {
        dbContext.Rentals.Update(rental);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}