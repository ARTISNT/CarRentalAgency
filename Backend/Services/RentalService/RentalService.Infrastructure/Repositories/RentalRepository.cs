using Microsoft.EntityFrameworkCore;
using RentalService.Domain.Rentals;
using RentalService.Domain.Rentals.Enums;
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
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Rental>> GetScheduledReadyRentalsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Rentals
            .Where(r => r.ActivityStatus == RentActivityStatus.Scheduled && r.StartDate <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Rental>> GetExpiredAwaitingConfirmationRentalsAsync(CancellationToken cancellationToken = default)
    {
        var expirationThreshold = DateTime.UtcNow.AddMinutes(-30);
        return await dbContext.Rentals
            .Where(r => r.ActivityStatus == RentActivityStatus.AwaitingConfirmation && r.CreatedAtUtc <= expirationThreshold)
            .ToListAsync(cancellationToken);
    }
}