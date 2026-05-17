using Microsoft.EntityFrameworkCore;
using RentalService.Domain.Rentals;

namespace RentalService.Infrastructure.Repositories;

public class RentalRepository(RentalServiceContext dbContext) : IRentalRepository
{
    public async Task<IReadOnlyCollection<Rental>> GetRentalsAsync()
    {
        return await dbContext.Rentals.ToListAsync();
    }
    
    public async Task<Rental?> GetRentalAsync(Guid rentalId)
    {
        return await dbContext.Rentals.FirstOrDefaultAsync(r => r.Id == rentalId);
    }

    public async Task AddRentalAsync(Rental rental)
    {
        await dbContext.Rentals.AddAsync(rental);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateRentalAsync(Rental rental)
    {
        dbContext.Rentals.Update(rental);
        await dbContext.SaveChangesAsync();
    }
}