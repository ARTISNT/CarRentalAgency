namespace RentalService.Domain.Rentals;

public interface IRentalRepository
{
    public Task<IReadOnlyCollection<Rental>> GetRentalsAsync();
    public Task<Rental?> GetRentalAsync(Guid rentalId);
    public Task AddRentalAsync(Rental rental);
    public Task UpdateRentalAsync(Rental rental);
}